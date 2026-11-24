using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Specifications.Payments;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Payment;
using ECommerce.Domain.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public partial class PaymentServiceTests
{
    private (Payment payment, Order order, StripeWebhookEventDTO evt) ArrangeWebhook(string type, string paymentStatus = "paid")
    {
        var order = new Order { Id = Guid.NewGuid(), OrderStatus = OrderStatus.Pending, UserId = "owner" };
        var payment = new Payment { Id = Guid.NewGuid(), OrderId = order.Id, UserId = "owner", Currency = "USD",
            Amount = 12.34m, AmountInMinorUnits = 1234, Status = PaymentStatus.Pending, StripeCheckoutSessionId = "cs_test", StripePaymentIntentId = "pi_test" };
        var evt = new StripeWebhookEventDTO { EventId = "evt_test", EventType = type, ClientReferenceId = payment.Id.ToString(),
            PaymentId = payment.Id.ToString(), OrderId = order.Id.ToString(), CheckoutSessionId = "cs_test", PaymentIntentId = "pi_test",
            Currency = "usd", AmountTotal = 1234, PaymentStatus = paymentStatus, CheckoutSessionStatus = "complete" };
        _paymentRepositoryMock.Setup(x => x.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _paymentGatewayMock.Setup(x => x.ParseWebhookEvent("payload", "signature")).Returns(evt);
        return (payment, order, evt);
    }

    [Fact]
    public async Task CompletedButUnpaid_MustNotConfirmOrder()
    {
        var (payment, order, _) = ArrangeWebhook("checkout.session.completed", "unpaid");
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        payment.Status.Should().Be(PaymentStatus.Processing);
        order.OrderStatus.Should().Be(OrderStatus.Pending);
        order.OrderStatusHistories.Should().BeEmpty();
    }

    [Theory]
    [InlineData("checkout.session.completed")]
    [InlineData("checkout.session.async_payment_succeeded")]
    public async Task MatchingPaidEvent_ConfirmsOnce(string type)
    {
        var (payment, order, _) = ArrangeWebhook(type);
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        payment.Status.Should().Be(PaymentStatus.Paid);
        order.OrderStatus.Should().Be(OrderStatus.Confirmed);
        order.OrderStatusHistories.Should().ContainSingle();
    }

    [Theory]
    [InlineData("amount")][InlineData("currency")][InlineData("reference")][InlineData("metadata-payment")]
    [InlineData("metadata-order")][InlineData("session")][InlineData("intent")]
    public async Task MismatchedFinancialIdentity_RejectsWithoutMutation(string mismatch)
    {
        var (payment, order, evt) = ArrangeWebhook("checkout.session.completed");
        switch (mismatch)
        {
            case "amount": evt.AmountTotal++; break;
            case "currency": evt.Currency = "eur"; break;
            case "reference": evt.ClientReferenceId = Guid.NewGuid().ToString(); break;
            case "metadata-payment": evt.PaymentId = Guid.NewGuid().ToString(); break;
            case "metadata-order": evt.OrderId = Guid.NewGuid().ToString(); break;
            case "session": evt.CheckoutSessionId = "cs_wrong"; break;
            case "intent": evt.PaymentIntentId = "pi_wrong"; break;
        }
        // Wrong reference still finds candidate by its session, then must reject the reference mismatch.
        _paymentRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<PaymentByCheckoutSessionSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        var act = () => _paymentService.HandleStripeWebhookAsync("payload", "signature");
        await act.Should().ThrowAsync<BadRequestException>();
        payment.Status.Should().Be(PaymentStatus.Pending);
        order.OrderStatus.Should().Be(OrderStatus.Pending);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(PaymentStatus.Paid)][InlineData(PaymentStatus.Refunded)][InlineData(PaymentStatus.Failed)][InlineData(PaymentStatus.Expired)]
    public async Task TerminalPayment_CannotBeResurrected(PaymentStatus status)
    {
        var (payment, order, _) = ArrangeWebhook("checkout.session.completed");
        payment.Status = status;
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        payment.Status.Should().Be(status);
        order.OrderStatus.Should().Be(OrderStatus.Pending);
    }

    [Theory]
    [InlineData(OrderStatus.Cancelled)][InlineData(OrderStatus.Refunded)][InlineData(OrderStatus.Shipped)]
    public async Task LatePaidEvent_DoesNotResurrectOrRegressOrder(OrderStatus status)
    {
        var (payment, order, _) = ArrangeWebhook("checkout.session.completed");
        order.OrderStatus = status;
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        payment.Status.Should().Be(PaymentStatus.Paid);
        order.OrderStatus.Should().Be(status);
    }

    [Fact]
    public async Task DuplicateEvent_IsAcknowledgedWithoutWriting()
    {
        ArrangeWebhook("checkout.session.completed");
        _stripeEventRepositoryMock.Setup(x => x.ExistsAsync(It.IsAny<StripeWebhookEventByEventIdSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UnknownEvent_IsAcknowledgedWithoutWriting()
    {
        ArrangeWebhook("customer.created");
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        _paymentRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("checkout.session.completed", PaymentStatus.Pending)]
    [InlineData("checkout.session.completed", PaymentStatus.Processing)]
    [InlineData("checkout.session.completed", PaymentStatus.Failed)]
    [InlineData("checkout.session.completed", PaymentStatus.Expired)]
    [InlineData("checkout.session.async_payment_succeeded", PaymentStatus.Pending)]
    [InlineData("checkout.session.async_payment_succeeded", PaymentStatus.Processing)]
    [InlineData("checkout.session.async_payment_succeeded", PaymentStatus.Failed)]
    [InlineData("checkout.session.async_payment_succeeded", PaymentStatus.Expired)]
    public async Task CancelledOrder_VerifiedLateMoney_IsPaidAndDurablyFlagged(string type, PaymentStatus previous)
    {
        var (payment, order, _) = ArrangeWebhook(type);
        order.OrderStatus = OrderStatus.Cancelled;
        payment.Status = previous;
        var records = new List<StripeWebhookEvent>();
        _stripeEventRepositoryMock.Setup(x => x.AddAsync(It.IsAny<StripeWebhookEvent>(), It.IsAny<CancellationToken>()))
            .Callback<StripeWebhookEvent, CancellationToken>((e, _) => records.Add(e)).Returns(Task.CompletedTask);

        await _paymentService.HandleStripeWebhookAsync("payload", "signature");

        payment.Status.Should().Be(PaymentStatus.Paid);
        payment.PaidAt.Should().NotBeNull();
        payment.AmountInMinorUnits.Should().Be(1234);
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        order.OrderStatusHistories.Should().BeEmpty();
        _orderRepositoryMock.Verify(x => x.Update(It.IsAny<Order>()), Times.Never);
        records.Should().ContainSingle(e => e.ProcessingResult == "ManualReviewRequired" && e.PaymentId == payment.Id);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _paymentGatewayMock.Verify(x => x.ParseWebhookEvent("payload", "signature"), Times.Once);
        _paymentGatewayMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelledOrder_RepeatAndFailureAfterPaid_KeepReviewAndFinancialTruth()
    {
        var (payment, order, evt) = ArrangeWebhook("checkout.session.completed");
        order.OrderStatus = OrderStatus.Cancelled;
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        var paidAt = payment.PaidAt;
        evt.EventId = "evt_second_success";
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        _stripeEventRepositoryMock.Verify(x => x.AddAsync(It.Is<StripeWebhookEvent>(e =>
            e.EventId == "evt_second_success" && e.ProcessingResult == "ManualReviewRequired"), It.IsAny<CancellationToken>()), Times.Once);
        evt.EventId = "evt_late_failure";
        evt.EventType = "checkout.session.async_payment_failed";
        evt.PaymentStatus = "unpaid";
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        payment.Status.Should().Be(PaymentStatus.Paid);
        payment.PaidAt.Should().Be(paidAt);
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        order.OrderStatusHistories.Should().BeEmpty();
    }

    [Theory]
    [InlineData("unpaid", PaymentStatus.Pending)]
    [InlineData("paid", PaymentStatus.Refunded)]
    public async Task CancelledOrder_WithoutEligiblePaidEvidence_DoesNotFlagReview(string evidence, PaymentStatus previous)
    {
        var (payment, order, _) = ArrangeWebhook("checkout.session.completed", evidence);
        order.OrderStatus = OrderStatus.Cancelled;
        payment.Status = previous;
        await _paymentService.HandleStripeWebhookAsync("payload", "signature");
        _stripeEventRepositoryMock.Verify(x => x.AddAsync(It.Is<StripeWebhookEvent>(e =>
            e.ProcessingResult == "ManualReviewRequired"), It.IsAny<CancellationToken>()), Times.Never);
        payment.Status.Should().Be(previous == PaymentStatus.Pending ? PaymentStatus.Processing : previous);
        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task CancelledOrder_MismatchedLateSuccess_DoesNotMarkPaidOrReview()
    {
        var (payment, order, evt) = ArrangeWebhook("checkout.session.async_payment_succeeded");
        order.OrderStatus = OrderStatus.Cancelled;
        payment.Status = PaymentStatus.Expired;
        evt.AmountTotal++;
        await FluentActions.Awaiting(() => _paymentService.HandleStripeWebhookAsync("payload", "signature"))
            .Should().ThrowAsync<BadRequestException>();
        payment.Status.Should().Be(PaymentStatus.Expired);
        _stripeEventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<StripeWebhookEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private Order ArrangeCheckout(Payment? existing = null)
    {
        var order = new Order { Id = existing?.OrderId ?? Guid.NewGuid(), UserId = "owner", Currency = "USD", TotalAmount = 12.34m, OrderStatus = OrderStatus.Pending };
        _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<CreateCheckoutSessionDTO>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _orderRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<ECommerce.Application.Specifications.Orders.OrderByUserSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _paymentRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<LatestPaymentByOrderSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _paymentGatewayMock.Setup(x => x.CreateCheckoutSessionAsync(It.IsAny<Payment>(), order, It.IsAny<CancellationToken>())).ReturnsAsync(new CreateCheckoutSessionResponseDTO { CheckoutSessionId = "cs_test", CheckoutSessionStatus = "open", CheckoutSessionUrl = "https://checkout.stripe.com/test" });
        return order;
    }

    [Fact]
    public async Task IncompleteAttempt_RetriesSameIdentityInsteadOfCreatingAnother()
    {
        var previous = new Payment { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Status = PaymentStatus.Pending, TransactionId = "stable-operation", Currency = "USD", AmountInMinorUnits = 1234 };
        var order = ArrangeCheckout(previous);
        await _paymentService.CreateCheckoutSessionAsync(order.Id, "owner");
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Never);
        _paymentGatewayMock.Verify(x => x.CreateCheckoutSessionAsync(previous, order, It.IsAny<CancellationToken>()), Times.Once);
        previous.TransactionId.Should().Be("stable-operation");
    }

    [Fact]
    public async Task ExpiredAttempt_CreatesNewIdentityAndKeepsOldHistory()
    {
        var previous = new Payment { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), Status = PaymentStatus.Expired, TransactionId = "old-operation" };
        var order = ArrangeCheckout(previous);
        await _paymentService.CreateCheckoutSessionAsync(order.Id, "owner");
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.Is<Payment>(p => p.Id != previous.Id && p.TransactionId != previous.TransactionId), It.IsAny<CancellationToken>()), Times.Once);
        previous.Status.Should().Be(PaymentStatus.Expired);
        previous.TransactionId.Should().Be("old-operation");
    }

    [Fact]
    public async Task StaleLocalOpenSession_IsNotBlindlyReturned()
    {
        var previous = new Payment { Id = Guid.NewGuid(), OrderId = Guid.NewGuid(), Status = PaymentStatus.Pending, Currency = "USD", AmountInMinorUnits = 1234,
            StripeCheckoutSessionId = "cs_test", StripeCheckoutSessionUrl = "https://checkout.stripe.com/test", StripeCheckoutSessionStatus = "open" };
        var order = ArrangeCheckout(previous);
        _paymentGatewayMock.Setup(x => x.GetCheckoutSessionAsync("cs_test", It.IsAny<CancellationToken>())).ReturnsAsync(new CreateCheckoutSessionResponseDTO { CheckoutSessionStatus = "complete", Status = "paid", AmountInMinorUnits = 1234, Currency = "usd" });
        var act = () => _paymentService.CreateCheckoutSessionAsync(order.Id, "owner");
        await act.Should().ThrowAsync<ConflictException>();
        _paymentGatewayMock.Verify(x => x.CreateCheckoutSessionAsync(It.IsAny<Payment>(), order, It.IsAny<CancellationToken>()), Times.Never);
    }
}

public partial class PaymentServiceTests
{
    [Fact]
    public async Task CancelledCheckout_StopsBeforeValidationOrPersistence()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var act = () => _paymentService.CreateCheckoutSessionAsync(Guid.NewGuid(), "owner", source.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<CreateCheckoutSessionDTO>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Checkout_CarriesRequestCancellationThroughEveryBoundary()
    {
        using var source = new CancellationTokenSource();
        var order = ArrangeCheckout();
        await _paymentService.CreateCheckoutSessionAsync(order.Id, "owner", source.Token);
        _validatorMock.Verify(x => x.ValidateAsync(It.IsAny<CreateCheckoutSessionDTO>(), source.Token), Times.Once);
        _orderRepositoryMock.Verify(x => x.GetFirstOrDefaultAsync(It.IsAny<ECommerce.Application.Specifications.Orders.OrderByUserSpecification>(), source.Token), Times.Once);
        _paymentRepositoryMock.Verify(x => x.GetFirstOrDefaultAsync(It.IsAny<LatestPaymentByOrderSpecification>(), source.Token), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Payment>(), source.Token), Times.Once);
        _paymentGatewayMock.Verify(x => x.CreateCheckoutSessionAsync(It.IsAny<Payment>(), order, source.Token), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(source.Token), Times.Exactly(2));
    }

    [Fact]
    public async Task Webhook_CarriesRequestCancellationThroughEveryBoundary()
    {
        using var source = new CancellationTokenSource();
        var (payment, order, _) = ArrangeWebhook("checkout.session.completed");
        await _paymentService.HandleStripeWebhookAsync("payload", "signature", source.Token);
        _stripeEventRepositoryMock.Verify(x => x.ExistsAsync(It.IsAny<StripeWebhookEventByEventIdSpecification>(), source.Token), Times.Once);
        _paymentRepositoryMock.Verify(x => x.GetByIdAsync(payment.Id, source.Token), Times.Once);
        _orderRepositoryMock.Verify(x => x.GetByIdAsync(order.Id, source.Token), Times.Once);
        _stripeEventRepositoryMock.Verify(x => x.AddAsync(It.IsAny<StripeWebhookEvent>(), source.Token), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(source.Token), Times.Once);
    }

    [Fact]
    public async Task CancellationDuringOrderRead_DoesNotStartPaymentOperation()
    {
        using var source = new CancellationTokenSource();
        var order = ArrangeCheckout();
        _orderRepositoryMock.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<ECommerce.Application.Specifications.Orders.OrderByUserSpecification>(), source.Token))
            .ThrowsAsync(new OperationCanceledException(source.Token));
        var act = () => _paymentService.CreateCheckoutSessionAsync(order.Id, "owner", source.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
        _paymentGatewayMock.Verify(x => x.CreateCheckoutSessionAsync(It.IsAny<Payment>(), It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
