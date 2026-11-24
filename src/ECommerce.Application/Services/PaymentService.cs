using AutoMapper;
using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Application.Specifications.Payments;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Payment;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services;

public class PaymentService : IPaymentService
{
    private const string StripeProviderName = "Stripe";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCheckoutSessionDTO> _createCheckoutValidator;
    private readonly IPaymentGatewayService _paymentGatewayService;

    public PaymentService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateCheckoutSessionDTO> createCheckoutValidator,
        IPaymentGatewayService paymentGatewayService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createCheckoutValidator = createCheckoutValidator;
        _paymentGatewayService = paymentGatewayService;
    }

    public async Task<CreateCheckoutSessionResponseDTO> CreateCheckoutSessionAsync(Guid orderId, string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var validationResult = await _createCheckoutValidator.ValidateAsync(new CreateCheckoutSessionDTO { OrderId = orderId }, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var order = await GetOwnedOrderAsync(orderId, userId, cancellationToken);

        if (order.OrderStatus is OrderStatus.Cancelled or OrderStatus.Refunded)
        {
            throw new BadRequestException("This order can no longer be paid.");
        }

        if (order.OrderStatus != OrderStatus.Pending)
            throw new BadRequestException("Only pending orders are eligible for checkout.");

        var existingPayment = await GetLatestPaymentForOrderAsync(orderId, cancellationToken);

        if (existingPayment is not null)
        {
            if (existingPayment.Status == PaymentStatus.Paid)
            {
                throw new BadRequestException("This order has already been paid.");
            }

            if (IsReusableCheckoutSession(existingPayment))
            {
                var remote = await _paymentGatewayService.GetCheckoutSessionAsync(existingPayment.StripeCheckoutSessionId!, cancellationToken);
                if (remote.AmountInMinorUnits != existingPayment.AmountInMinorUnits
                    || !string.Equals(remote.Currency, existingPayment.Currency, StringComparison.OrdinalIgnoreCase))
                    throw new ConflictException("Provider session does not match the payment expectation.");
                if (remote.CheckoutSessionStatus == "open" && remote.Status != "paid")
                    return ToCheckoutResponse(existingPayment);
                // Only the signed webhook can confirm money. Never create another session while one completed.
                throw new ConflictException("The provider session is no longer open. Wait for its webhook before retrying.");
            }
        }

        if (existingPayment?.Status == PaymentStatus.Processing)
            throw new ConflictException("This payment is still processing. Wait for the provider result.");

        if (existingPayment?.Status == PaymentStatus.Refunded)
            throw new BadRequestException("A refunded order cannot be paid again.");

        if (order.OrderStatus != OrderStatus.Pending)
            throw new BadRequestException("Only pending orders are eligible for checkout.");

        // A remote timeout or local save failure must retry the SAME provider operation.
        var recoverIncompleteAttempt = existingPayment?.Status == PaymentStatus.Pending
            && string.IsNullOrWhiteSpace(existingPayment.StripeCheckoutSessionId);
        var payment = recoverIncompleteAttempt ? existingPayment! : BuildNewPayment(order, userId);
        if (!recoverIncompleteAttempt)
        {
            await _unitOfWork.GetRepository<Payment, Guid>().AddAsync(payment, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        // Stripe idempotency retention is finite. Old unresolved operations require reconciliation, not a new charge.
        if (recoverIncompleteAttempt && payment.CreatedAt < DateTime.UtcNow.AddHours(-23))
            throw new ConflictException("An old checkout attempt needs provider reconciliation before retrying.");

        var checkoutSession = await _paymentGatewayService.CreateCheckoutSessionAsync(payment, order, cancellationToken);

        payment.PaymentGateway = StripeProviderName;
        payment.Status = PaymentStatus.Pending;
        payment.Method = PaymentMethod.Stripe;
        payment.StripeCheckoutSessionId = checkoutSession.CheckoutSessionId;
        payment.StripeCheckoutSessionStatus = checkoutSession.CheckoutSessionStatus;
        payment.StripeCheckoutSessionUrl = checkoutSession.CheckoutSessionUrl;
        payment.StripePaymentIntentId = checkoutSession.PaymentIntentId;
        payment.GatewayTransactionId = checkoutSession.PaymentIntentId;
        payment.GatewayResponse = checkoutSession.Status;

        _unitOfWork.GetRepository<Payment, Guid>().Update(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        checkoutSession.PaymentId = payment.Id;
        checkoutSession.OrderId = order.Id;
        checkoutSession.TransactionId = payment.TransactionId;
        checkoutSession.Amount = payment.Amount;
        checkoutSession.AmountInMinorUnits = payment.AmountInMinorUnits;
        checkoutSession.Currency = payment.Currency;
        checkoutSession.Status = payment.Status.ToString();
        checkoutSession.CheckoutSessionStatus = payment.StripeCheckoutSessionStatus;

        return checkoutSession;
    }

    public async Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var order = await GetAccessibleOrderAsync(orderId, userId, isAdmin, cancellationToken);

        var spec = new PaymentsByOrderSpecification(order.Id);
        var payments = await _unitOfWork.GetRepository<Payment, Guid>().GetAllAsync(spec, cancellationToken);
        return _mapper.Map<IEnumerable<GetPaymentDTO>>(payments);
    }

    public async Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.GetRepository<Payment, Guid>().GetByIdAsync(id, cancellationToken);
        if (payment is null)
        {
            throw new NotFoundException($"Payment with ID {id} not found.");
        }

        if (!isAdmin && !string.Equals(payment.UserId, userId, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("You are not allowed to access this payment record.");
        }

        return _mapper.Map<GetPaymentDTO>(payment);
    }

    public async Task HandleStripeWebhookAsync(string payload, string signatureHeader, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var stripeEvent = _paymentGatewayService.ParseWebhookEvent(payload, signatureHeader);

        if (stripeEvent.EventType is not ("checkout.session.completed" or "checkout.session.async_payment_succeeded"
            or "checkout.session.async_payment_failed" or "checkout.session.expired"))
            return;

        if (await EventAlreadyProcessedAsync(stripeEvent.EventId, cancellationToken))
        {
            return;
        }

        var payment = await FindPaymentForStripeEventAsync(stripeEvent, cancellationToken);

        if (payment is null)
        {
            throw new NotFoundException("No matching payment record was found for the Stripe event.");
        }

        ValidateFinancialEvent(payment, stripeEvent);
        var now = DateTime.UtcNow;
        // Cancellation is a fulfillment decision, not proof that no money arrived.
        // Read the order for verified success, including a late success after local expiry/failure.
        var confirmedSuccess = (stripeEvent.EventType is "checkout.session.completed" or "checkout.session.async_payment_succeeded")
            && IsConfirmedPayment(stripeEvent);
        var order = confirmedSuccess && payment.Status != PaymentStatus.Refunded
            ? await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(payment.OrderId, cancellationToken)
            : null;
        var cancelledOrder = order?.OrderStatus == OrderStatus.Cancelled;
        var nextStatus = ResolveNextStatus(payment, stripeEvent, cancelledOrder);
        var manualReview = cancelledOrder && confirmedSuccess
            && (nextStatus == PaymentStatus.Paid || payment.Status == PaymentStatus.Paid);

        if (nextStatus is null)
        {
            await RecordStripeEventAsync(stripeEvent, payment, manualReview ? "ManualReviewRequired" : "Ignored", null, now, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        ApplyPaymentUpdate(payment, stripeEvent, nextStatus.Value, now);

        if (nextStatus == PaymentStatus.Paid)
        {
            ConfirmOrderIfNeeded(order, now);
        }

        await RecordStripeEventAsync(stripeEvent, payment, manualReview ? "ManualReviewRequired" : "Processed", null, now, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsDuplicateStripeWebhookEvent(ex))
        {
            return;
        }
    }

    private async Task<Order> GetOwnedOrderAsync(Guid orderId, string userId, CancellationToken cancellationToken)
    {
        var specification = new OrderByUserSpecification(orderId, userId);
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(specification, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException($"Order with ID {orderId} not found.");
        }

        return order;
    }

    private async Task<Order> GetAccessibleOrderAsync(Guid orderId, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        if (isAdmin)
        {
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId, cancellationToken);
            if (order is null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            return order;
        }

        return await GetOwnedOrderAsync(orderId, userId, cancellationToken);
    }

    private async Task<Payment?> GetLatestPaymentForOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.GetRepository<Payment, Guid>().GetFirstOrDefaultAsync(new LatestPaymentByOrderSpecification(orderId), cancellationToken);
    }

    private static Payment BuildNewPayment(Order order, string userId)
    {
        var amountInMinorUnits = ToMinorUnits(order.TotalAmount, order.Currency);
        if (amountInMinorUnits <= 0)
            throw new BadRequestException("This checkout requires a positive order total.");

        return new Payment
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OrderId = order.Id,
            UserId = userId,
            Amount = order.TotalAmount,
            AmountInMinorUnits = amountInMinorUnits,
            Currency = NormalizeCurrency(order.Currency),
            PaymentGateway = StripeProviderName,
            Method = PaymentMethod.Stripe,
            Status = PaymentStatus.Pending,
            TransactionId = BuildTransactionId(order.Id),
            PaidAt = null,
            StripeCheckoutSessionStatus = "open"
        };
    }

    private static bool IsReusableCheckoutSession(Payment payment)
    {
        return payment.Status == PaymentStatus.Pending
            && !string.IsNullOrWhiteSpace(payment.StripeCheckoutSessionId)
            && !string.IsNullOrWhiteSpace(payment.StripeCheckoutSessionUrl)
            && string.Equals(payment.StripeCheckoutSessionStatus, "open", StringComparison.OrdinalIgnoreCase);
    }

    private static CreateCheckoutSessionResponseDTO ToCheckoutResponse(Payment payment)
    {
        return new CreateCheckoutSessionResponseDTO
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            TransactionId = payment.TransactionId,
            CheckoutSessionId = payment.StripeCheckoutSessionId ?? string.Empty,
            CheckoutSessionUrl = payment.StripeCheckoutSessionUrl,
            PaymentIntentId = payment.StripePaymentIntentId,
            Amount = payment.Amount,
            AmountInMinorUnits = payment.AmountInMinorUnits,
            Currency = payment.Currency,
            Status = payment.Status.ToString(),
            CheckoutSessionStatus = payment.StripeCheckoutSessionStatus
        };
    }

    private static string BuildTransactionId(Guid orderId)
        => $"PAY-{orderId.ToString("N")[..8].ToUpperInvariant()}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private static string NormalizeCurrency(string currency)
    {
        if (!string.Equals(currency?.Trim(), "USD", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("The catalog is priced in USD. Other currencies are not supported.");
        return "USD";
    }

    private static long ToMinorUnits(decimal amount, string currency)
    {
        NormalizeCurrency(currency);
        if (amount < 0 || decimal.Round(amount, 2) != amount)
            throw new BadRequestException("USD amounts must be nonnegative and have at most two decimal places.");
        try { return checked((long)(amount * 100m)); }
        catch (OverflowException) { throw new BadRequestException("The order amount is too large."); }
    }

    private static void ValidateFinancialEvent(Payment payment, StripeWebhookEventDTO e)
    {
        if (!Guid.TryParse(e.ClientReferenceId, out var paymentId) || paymentId != payment.Id
            || !Guid.TryParse(e.OrderId, out var orderId) || orderId != payment.OrderId
            || !Guid.TryParse(e.PaymentId, out var metadataPaymentId) || metadataPaymentId != payment.Id
            || string.IsNullOrWhiteSpace(e.CheckoutSessionId)
            || !string.Equals(payment.StripeCheckoutSessionId, e.CheckoutSessionId, StringComparison.Ordinal)
            || (!string.IsNullOrEmpty(payment.StripePaymentIntentId)
                && !string.Equals(payment.StripePaymentIntentId, e.PaymentIntentId, StringComparison.Ordinal))
            || e.AmountTotal != payment.AmountInMinorUnits
            || !string.Equals(e.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Stripe event does not match the persisted payment expectation.");
    }

    private async Task<Payment?> FindPaymentForStripeEventAsync(StripeWebhookEventDTO stripeEvent, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(stripeEvent.ClientReferenceId, out var paymentId))
        {
            var byId = await _unitOfWork.GetRepository<Payment, Guid>().GetByIdAsync(paymentId, cancellationToken);
            if (byId is not null)
            {
                return byId;
            }
        }

        if (!string.IsNullOrWhiteSpace(stripeEvent.CheckoutSessionId))
        {
            var spec = new PaymentByCheckoutSessionSpecification(stripeEvent.CheckoutSessionId);
            var bySession = await _unitOfWork.GetRepository<Payment, Guid>().GetFirstOrDefaultAsync(spec, cancellationToken);
            if (bySession is not null)
            {
                return bySession;
            }
        }

        if (!string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
        {
            var spec = new PaymentByPaymentIntentSpecification(stripeEvent.PaymentIntentId);
            var byIntent = await _unitOfWork.GetRepository<Payment, Guid>().GetFirstOrDefaultAsync(spec, cancellationToken);
            if (byIntent is not null)
            {
                return byIntent;
            }
        }

        return null;
    }

    private static PaymentStatus? ResolveNextStatus(Payment payment, StripeWebhookEventDTO stripeEvent, bool cancelledOrder)
    {
        if (payment.Status is PaymentStatus.Paid or PaymentStatus.Refunded)
        {
            return null;
        }

        if (cancelledOrder && (payment.Status is PaymentStatus.Failed or PaymentStatus.Expired)
            && (stripeEvent.EventType is "checkout.session.completed" or "checkout.session.async_payment_succeeded")
            && IsConfirmedPayment(stripeEvent))
            return PaymentStatus.Paid;

        return stripeEvent.EventType switch
        {
            "checkout.session.completed" when payment.Status is PaymentStatus.Pending or PaymentStatus.Processing
                && IsConfirmedPayment(stripeEvent) => PaymentStatus.Paid,
            "checkout.session.completed" when payment.Status == PaymentStatus.Pending => PaymentStatus.Processing,
            "checkout.session.async_payment_succeeded" when payment.Status is PaymentStatus.Pending or PaymentStatus.Processing
                && IsConfirmedPayment(stripeEvent) => PaymentStatus.Paid,
            "checkout.session.async_payment_failed" when payment.Status is PaymentStatus.Pending or PaymentStatus.Processing => PaymentStatus.Failed,
            "checkout.session.expired" when payment.Status is PaymentStatus.Pending or PaymentStatus.Processing => PaymentStatus.Expired,
            _ => null
        };
    }

    private static bool IsConfirmedPayment(StripeWebhookEventDTO stripeEvent)
    {
        return string.Equals(stripeEvent.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);
    }

    private void ApplyPaymentUpdate(Payment payment, StripeWebhookEventDTO stripeEvent, PaymentStatus nextStatus, DateTime now)
    {
        payment.Status = nextStatus;
        payment.PaymentGateway = StripeProviderName;
        payment.StripeCheckoutSessionId = stripeEvent.CheckoutSessionId ?? payment.StripeCheckoutSessionId;
        payment.StripePaymentIntentId = stripeEvent.PaymentIntentId ?? payment.StripePaymentIntentId;
        payment.StripeCheckoutSessionStatus = stripeEvent.CheckoutSessionStatus ?? payment.StripeCheckoutSessionStatus;
        payment.GatewayTransactionId = stripeEvent.PaymentIntentId ?? payment.GatewayTransactionId;
        payment.GatewayResponse = stripeEvent.PayloadSummary;

        if (nextStatus == PaymentStatus.Paid)
        {
            payment.PaidAt = now;
            payment.StripeCheckoutSessionStatus = "complete";
        }
        else if (nextStatus == PaymentStatus.Expired)
        {
            payment.StripeCheckoutSessionStatus = "expired";
        }
        else if (nextStatus == PaymentStatus.Processing)
        {
            payment.StripeCheckoutSessionStatus ??= "complete";
        }

        _unitOfWork.GetRepository<Payment, Guid>().Update(payment);
    }

    private void ConfirmOrderIfNeeded(Order? order, DateTime now)
    {
        if (order is null)
        {
            return;
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            return;
        }

        order.OrderStatus = OrderStatus.Confirmed;
        order.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderStatus = OrderStatus.Confirmed,
            CreatedAt = now,
            UpdatedAt = now
        });

        _unitOfWork.GetRepository<Order, Guid>().Update(order);
    }

    private async Task<bool> EventAlreadyProcessedAsync(string eventId, CancellationToken cancellationToken)
    {
        var spec = new StripeWebhookEventByEventIdSpecification(eventId);
        return await _unitOfWork.GetRepository<StripeWebhookEvent, Guid>().ExistsAsync(spec, cancellationToken);
    }

    private async Task RecordStripeEventAsync(
        StripeWebhookEventDTO stripeEvent,
        Payment payment,
        string processingResult,
        string? errorMessage,
        DateTime now, CancellationToken cancellationToken)
    {
        var webhookEvent = new StripeWebhookEvent
        {
            EventId = stripeEvent.EventId,
            EventType = stripeEvent.EventType,
            Provider = StripeProviderName,
            PaymentId = payment.Id,
            StripeCheckoutSessionId = stripeEvent.CheckoutSessionId,
            StripePaymentIntentId = stripeEvent.PaymentIntentId,
            ProcessingResult = processingResult,
            ErrorMessage = errorMessage,
            ProcessedAt = now
        };

        await _unitOfWork.GetRepository<StripeWebhookEvent, Guid>().AddAsync(webhookEvent, cancellationToken);
    }

    private static bool IsDuplicateStripeWebhookEvent(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains("IX_StripeWebhookEvents_EventId", StringComparison.OrdinalIgnoreCase);
    }
}
