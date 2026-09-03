using AutoMapper;
using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Payments;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreatePaymentDTO>> _createValidatorMock;
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreatePaymentDTO>>();
        _paymentService = new PaymentService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    [Fact]
    public async Task GetPaymentsByOrderIdAsync_ShouldReturnMappedPayments()
    {
        var payments = new List<Payment> { new() { Id = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1) } };
        var paymentDtos = new List<GetPaymentDTO> { new() { Id = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1) } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Payment, Guid>().GetAllAsync(new PaymentsByOrderSpecification(TestGuid.FromInt(1))))
            .ReturnsAsync(payments);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetPaymentDTO>>(payments)).Returns(paymentDtos);

        var result = await _paymentService.GetPaymentsByOrderIdAsync(TestGuid.FromInt(1));

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenExists_ShouldReturnPayment()
    {
        var payment = new Payment { Id = TestGuid.FromInt(1), Amount = 99.99m };
        var paymentDto = new GetPaymentDTO { Id = TestGuid.FromInt(1), Amount = 99.99m };

        _unitOfWorkMock.Setup(u => u.GetRepository<Payment, Guid>().GetByIdAsync(TestGuid.FromInt(1)))
            .ReturnsAsync(payment);
        _mapperMock.Setup(m => m.Map<GetPaymentDTO>(payment)).Returns(paymentDto);

        var result = await _paymentService.GetPaymentByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.Amount.Should().Be(99.99m);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Payment, Guid>().GetByIdAsync(TestGuid.FromInt(999)))
            .ReturnsAsync((Payment?)null);

        var act = () => _paymentService.GetPaymentByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreatePaymentAsync_WithValidData_ShouldCreatePayment()
    {
        var dto = new CreatePaymentDTO { OrderId = TestGuid.FromInt(1), UserId = "user1", Amount = 99.99m, Method = "CreditCard", Currency = "USD" };
        var paymentDto = new GetPaymentDTO { Id = TestGuid.FromInt(1), Amount = 99.99m };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Payment, Guid>().AddAsync(It.IsAny<Payment>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetPaymentDTO>(It.IsAny<Payment>())).Returns(paymentDto);

        var result = await _paymentService.CreatePaymentAsync(dto);

        result.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.GetRepository<Payment, Guid>().AddAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task CreatePaymentAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreatePaymentDTO { OrderId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("OrderId", "OrderId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _paymentService.CreatePaymentAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
