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

    public async Task<CreateCheckoutSessionResponseDTO> CreateCheckoutSessionAsync(Guid orderId, string userId)
    {
        var validationResult = await _createCheckoutValidator.ValidateAsync(new CreateCheckoutSessionDTO { OrderId = orderId });
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var order = await GetOwnedOrderAsync(orderId, userId);

        if (order.OrderStatus is OrderStatus.Cancelled or OrderStatus.Refunded)
        {
            throw new BadRequestException("This order can no longer be paid.");
        }

        var existingPayment = await GetLatestPaymentForOrderAsync(orderId);

        if (existingPayment is not null)
        {
            if (existingPayment.Status == PaymentStatus.Paid)
            {
                throw new BadRequestException("This order has already been paid.");
            }

            if (IsReusableCheckoutSession(existingPayment))
            {
                return ToCheckoutResponse(existingPayment);
            }
        }

        var payment = existingPayment is null
            ? BuildNewPayment(order, userId)
            : existingPayment;

        if (existingPayment is null)
        {
            await _unitOfWork.GetRepository<Payment, Guid>().AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
        }

        var checkoutSession = await _paymentGatewayService.CreateCheckoutSessionAsync(payment, order);

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
        await _unitOfWork.SaveChangesAsync();

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

    public async Task<IEnumerable<GetPaymentDTO>> GetPaymentsByOrderIdAsync(Guid orderId, string userId, bool isAdmin)
    {
        var order = await GetAccessibleOrderAsync(orderId, userId, isAdmin);

        var spec = new PaymentsByOrderSpecification(order.Id);
        var payments = await _unitOfWork.GetRepository<Payment, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetPaymentDTO>>(payments);
    }

    public async Task<GetPaymentDTO> GetPaymentByIdAsync(Guid id, string userId, bool isAdmin)
    {
        var payment = await _unitOfWork.GetRepository<Payment, Guid>().GetByIdAsync(id);
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

    public async Task HandleStripeWebhookAsync(string payload, string signatureHeader)
    {
        var stripeEvent = _paymentGatewayService.ParseWebhookEvent(payload, signatureHeader);

        if (await EventAlreadyProcessedAsync(stripeEvent.EventId))
        {
            return;
        }

        var payment = await FindPaymentForStripeEventAsync(stripeEvent);

        if (payment is null)
        {
            throw new NotFoundException("No matching payment record was found for the Stripe event.");
        }

        var now = DateTime.UtcNow;
        var nextStatus = ResolveNextStatus(payment, stripeEvent);

        if (nextStatus is null)
        {
            await RecordStripeEventAsync(stripeEvent, payment, "Ignored", null, now);
            await _unitOfWork.SaveChangesAsync();
            return;
        }

        ApplyPaymentUpdate(payment, stripeEvent, nextStatus.Value, now);

        if (nextStatus == PaymentStatus.Paid)
        {
            await ConfirmOrderIfNeededAsync(payment.OrderId, now);
        }

        await RecordStripeEventAsync(stripeEvent, payment, "Processed", null, now);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsDuplicateStripeWebhookEvent(ex))
        {
            return;
        }
    }

    private async Task<Order> GetOwnedOrderAsync(Guid orderId, string userId)
    {
        var specification = new OrderByUserSpecification(orderId, userId);
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(specification);

        if (order is null)
        {
            throw new NotFoundException($"Order with ID {orderId} not found.");
        }

        return order;
    }

    private async Task<Order> GetAccessibleOrderAsync(Guid orderId, string userId, bool isAdmin)
    {
        if (isAdmin)
        {
            var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId);
            if (order is null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.");
            }

            return order;
        }

        return await GetOwnedOrderAsync(orderId, userId);
    }

    private async Task<Payment?> GetLatestPaymentForOrderAsync(Guid orderId)
    {
        var spec = new PaymentsByOrderSpecification(orderId);
        var payments = await _unitOfWork.GetRepository<Payment, Guid>().GetAllAsync(spec);
        return payments.FirstOrDefault();
    }

    private static Payment BuildNewPayment(Order order, string userId)
    {
        var amountInMinorUnits = ToMinorUnits(order.TotalAmount, order.Currency);

        return new Payment
        {
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
        return payment.Status is PaymentStatus.Pending or PaymentStatus.Processing
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
        => string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();

    private static long ToMinorUnits(decimal amount, string currency)
    {
        var normalizedCurrency = NormalizeCurrency(currency);
        var minorUnitFactor = UsesZeroDecimalCurrency(normalizedCurrency) ? 1m : 100m;
        return Convert.ToInt64(decimal.Round(amount * minorUnitFactor, 0, MidpointRounding.AwayFromZero));
    }

    private static bool UsesZeroDecimalCurrency(string currency)
    {
        return currency is "BIF" or "CLP" or "DJF" or "GNF" or "ISK" or "JPY" or "KMF" or "KRW" or "PYG" or "RWF" or "UGX" or "VND" or "VUV" or "XAF" or "XOF" or "XPF";
    }

    private async Task<Payment?> FindPaymentForStripeEventAsync(StripeWebhookEventDTO stripeEvent)
    {
        if (Guid.TryParse(stripeEvent.ClientReferenceId, out var paymentId))
        {
            var byId = await _unitOfWork.GetRepository<Payment, Guid>().GetByIdAsync(paymentId);
            if (byId is not null)
            {
                return byId;
            }
        }

        if (!string.IsNullOrWhiteSpace(stripeEvent.CheckoutSessionId))
        {
            var spec = new PaymentByCheckoutSessionSpecification(stripeEvent.CheckoutSessionId);
            var bySession = await _unitOfWork.GetRepository<Payment, Guid>().GetFirstOrDefaultAsync(spec);
            if (bySession is not null)
            {
                return bySession;
            }
        }

        if (!string.IsNullOrWhiteSpace(stripeEvent.PaymentIntentId))
        {
            var spec = new PaymentByPaymentIntentSpecification(stripeEvent.PaymentIntentId);
            var byIntent = await _unitOfWork.GetRepository<Payment, Guid>().GetFirstOrDefaultAsync(spec);
            if (byIntent is not null)
            {
                return byIntent;
            }
        }

        return null;
    }

    private static PaymentStatus? ResolveNextStatus(Payment payment, StripeWebhookEventDTO stripeEvent)
    {
        if (payment.Status == PaymentStatus.Paid)
        {
            return null;
        }

        return stripeEvent.EventType switch
        {
            "checkout.session.completed" when IsConfirmedPayment(stripeEvent) => PaymentStatus.Paid,
            "checkout.session.completed" => PaymentStatus.Processing,
            "checkout.session.async_payment_succeeded" => PaymentStatus.Paid,
            "checkout.session.async_payment_failed" => PaymentStatus.Failed,
            "checkout.session.expired" when payment.Status is PaymentStatus.Pending or PaymentStatus.Processing => PaymentStatus.Expired,
            _ => null
        };
    }

    private static bool IsConfirmedPayment(StripeWebhookEventDTO stripeEvent)
    {
        return string.Equals(stripeEvent.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase)
            || string.Equals(stripeEvent.CheckoutSessionStatus, "complete", StringComparison.OrdinalIgnoreCase);
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

    private async Task ConfirmOrderIfNeededAsync(Guid orderId, DateTime now)
    {
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(orderId);
        if (order is null)
        {
            return;
        }

        if (order.OrderStatus == OrderStatus.Confirmed)
        {
            return;
        }

        if (order.OrderStatus is OrderStatus.Cancelled or OrderStatus.Refunded)
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

    private async Task<bool> EventAlreadyProcessedAsync(string eventId)
    {
        var spec = new StripeWebhookEventByEventIdSpecification(eventId);
        return await _unitOfWork.GetRepository<StripeWebhookEvent, Guid>().ExistsAsync(spec);
    }

    private async Task RecordStripeEventAsync(
        StripeWebhookEventDTO stripeEvent,
        Payment payment,
        string processingResult,
        string? errorMessage,
        DateTime now)
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

        await _unitOfWork.GetRepository<StripeWebhookEvent, Guid>().AddAsync(webhookEvent);
    }

    private static bool IsDuplicateStripeWebhookEvent(DbUpdateException exception)
    {
        var message = exception.InnerException?.Message ?? exception.Message;
        return message.Contains("IX_StripeWebhookEvents_EventId", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || message.Contains("2601", StringComparison.OrdinalIgnoreCase)
            || message.Contains("2627", StringComparison.OrdinalIgnoreCase);
    }
}