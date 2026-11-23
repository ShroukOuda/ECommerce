using AutoMapper;
using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Payment;
using ECommerce.Domain.Exceptions;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ECommerce.Application.Interfaces;

namespace ECommerce.UnitTests.Services;

public partial class PaymentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IValidator<CreateCheckoutSessionDTO>> _validatorMock = new();
    private readonly Mock<IPaymentGatewayService> _paymentGatewayMock = new();
    private readonly Mock<IGenericRepository<Order, Guid>> _orderRepositoryMock = new();
    private readonly Mock<IGenericRepository<Payment, Guid>> _paymentRepositoryMock = new();
    private readonly Mock<IGenericRepository<StripeWebhookEvent, Guid>> _stripeEventRepositoryMock = new();
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>()).Returns(_orderRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Payment, Guid>()).Returns(_paymentRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<StripeWebhookEvent, Guid>()).Returns(_stripeEventRepositoryMock.Object);

        _paymentService = new PaymentService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _validatorMock.Object,
            _paymentGatewayMock.Object);
    }

    [Fact]
    public async Task GetPaymentsByOrderIdAsync_ShouldReturnMappedPayments()
    {
        var order = new Order { Id = TestGuid.FromInt(1), UserId = "user-123", OrderStatus = OrderStatus.Pending };
        var payments = new List<Payment> { new() { Id = TestGuid.FromInt(1), OrderId = order.Id, UserId = order.UserId } };
        var paymentDtos = new List<GetPaymentDTO> { new() { Id = TestGuid.FromInt(1), OrderId = order.Id, UserId = order.UserId } };

        _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ECommerce.Domain.Specifications.Base.BaseSpecification<Order, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<ECommerce.Domain.Specifications.Base.BaseSpecification<Payment, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payments);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetPaymentDTO>>(payments)).Returns(paymentDtos);

        var result = await _paymentService.GetPaymentsByOrderIdAsync(order.Id, order.UserId, false);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenExistsAndOwned_ShouldReturnPayment()
    {
        var payment = new Payment { Id = TestGuid.FromInt(1), Amount = 99.99m, UserId = "user-123" };
        var paymentDto = new GetPaymentDTO { Id = TestGuid.FromInt(1), Amount = 99.99m, UserId = "user-123" };

        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        _mapperMock.Setup(m => m.Map<GetPaymentDTO>(payment)).Returns(paymentDto);

        var result = await _paymentService.GetPaymentByIdAsync(payment.Id, payment.UserId, false);

        result.Should().NotBeNull();
        result.Amount.Should().Be(99.99m);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(TestGuid.FromInt(999), It.IsAny<CancellationToken>())).ReturnsAsync((Payment?)null);

        var act = () => _paymentService.GetPaymentByIdAsync(TestGuid.FromInt(999), "user-123", false);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenAccessingAnotherUsersPayment_ShouldThrowUnauthorizedAccessException()
    {
        var payment = new Payment { Id = TestGuid.FromInt(1), UserId = "other-user" };
        _paymentRepositoryMock.Setup(r => r.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);

        var act = () => _paymentService.GetPaymentByIdAsync(payment.Id, "user-123", false);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_WithValidData_ShouldCreateSession()
    {
        var order = new Order
        {
            Id = TestGuid.FromInt(1),
            UserId = "user-123",
            OrderNumber = "ORD-1",
            TotalAmount = 99.99m,
            Currency = "USD",
            OrderStatus = OrderStatus.Pending,
            OrderStatusHistories = new List<OrderStatusHistory>()
        };

        var payment = new Payment
        {
            Id = TestGuid.FromInt(2),
            OrderId = order.Id,
            UserId = order.UserId,
            Amount = 99.99m,
            Currency = "USD",
            TransactionId = "PAY-00000001-00000002",
            Status = PaymentStatus.Pending,
            Method = PaymentMethod.Stripe,
            AmountInMinorUnits = 9999
        };

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateCheckoutSessionDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _orderRepositoryMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<ECommerce.Domain.Specifications.Base.BaseSpecification<Order, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _paymentRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<ECommerce.Domain.Specifications.Base.BaseSpecification<Payment, Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Payment>());
        _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _paymentGatewayMock.Setup(g => g.CreateCheckoutSessionAsync(It.IsAny<Payment>(), order, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateCheckoutSessionResponseDTO
            {
                PaymentId = payment.Id,
                OrderId = order.Id,
                TransactionId = payment.TransactionId,
                CheckoutSessionId = "cs_test_123",
                CheckoutSessionUrl = "https://checkout.stripe.com/pay/cs_test_123",
                PaymentIntentId = "pi_test_123",
                Amount = payment.Amount,
                AmountInMinorUnits = payment.AmountInMinorUnits,
                Currency = payment.Currency,
                Status = "open",
                CheckoutSessionStatus = "open"
            });

        var result = await _paymentService.CreateCheckoutSessionAsync(order.Id, order.UserId);

        result.CheckoutSessionId.Should().Be("cs_test_123");
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_WithInvalidData_ShouldThrowValidationException()
    {
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CreateCheckoutSessionDTO>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new("OrderId", "OrderId required") }));

        var act = () => _paymentService.CreateCheckoutSessionAsync(Guid.Empty, "user-123");

        await act.Should().ThrowAsync<ValidationException>();
    }
}
