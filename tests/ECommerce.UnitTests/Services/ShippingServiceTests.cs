using ECommerce.Domain.Enums.Shipping;
using AutoMapper;
using ECommerce.Application.DTO.Shipping;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Shippings;
using ECommerce.Domain.Entities.Shippings;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Payment;
using ECommerce.Domain.Exceptions;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ShippingServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateShippingDTO>> _createValidatorMock;
    private readonly IShippingService _shippingService;

    public ShippingServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateShippingDTO>>();
        _shippingService = new ShippingService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    [Fact]
    public async Task GetShippingsByOrderIdAsync_ShouldReturnMappedShippings()
    {
        var shippings = new List<Shipping> { new() { Id = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1) } };
        var shippingDtos = new List<GetShippingDTO> { new() { Id = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1) } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().GetAllAsync(It.IsAny<ShippingsByOrderSpecification>()))
            .ReturnsAsync(shippings);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetShippingDTO>>(shippings)).Returns(shippingDtos);

        var result = await _shippingService.GetShippingsByOrderIdAsync(TestGuid.FromInt(1));

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetShippingByIdAsync_WhenExists_ShouldReturnShipping()
    {
        var shipping = new Shipping { Id = TestGuid.FromInt(1), TrackingNumber = "SHP-001" };
        var shippingDto = new GetShippingDTO { Id = TestGuid.FromInt(1), TrackingNumber = "SHP-001" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(It.IsAny<ShippingByIdSpecification>()))
            .ReturnsAsync(shipping);
        _mapperMock.Setup(m => m.Map<GetShippingDTO>(shipping)).Returns(shippingDto);

        var result = await _shippingService.GetShippingByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.TrackingNumber.Should().Be("SHP-001");
    }

    [Fact]
    public async Task GetShippingByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(It.IsAny<ShippingByIdSpecification>()))
            .ReturnsAsync((Shipping?)null);

        var act = () => _shippingService.GetShippingByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    private static Order ProcessingPaidOrder() => new()
    {
        Id = TestGuid.FromInt(1),
        OrderStatus = OrderStatus.Processing,
        ShippingAddressId = TestGuid.FromInt(1),
        Payments = new List<Payment> { new() { Status = PaymentStatus.Paid } }
    };

    private void SetupOrder(Order? order)
        => _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);

    private CreateShippingDTO ValidCreateDto()
    {
        var dto = new CreateShippingDTO { OrderId = TestGuid.FromInt(1), AddressId = TestGuid.FromInt(1), Method = ShippingMethod.Standard, Cost = 9.99m };
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<Shipping>(dto)).Returns(new Shipping { OrderId = dto.OrderId, AddressId = dto.AddressId });
        _mapperMock.Setup(m => m.Map<GetShippingDTO>(It.IsAny<Shipping>())).Returns(new GetShippingDTO());
        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().AddAsync(It.IsAny<Shipping>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        return dto;
    }

    [Fact]
    public async Task CreateShippingAsync_WithValidData_CreatesPendingShipmentWithHistory()
    {
        var dto = ValidCreateDto();
        SetupOrder(ProcessingPaidOrder());
        Shipping? saved = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().AddAsync(It.IsAny<Shipping>()))
            .Callback<Shipping>(x => saved = x).Returns(Task.CompletedTask);

        await _shippingService.CreateShippingAsync(dto, "admin-1");

        saved!.Status.Should().Be(ShippingStatus.Pending);
        var history = saved.StatusHistories.Should().ContainSingle().Subject;
        history.ShippingStatus.Should().Be(ShippingStatus.Pending);
        history.Source.Should().Be("Admin");
        history.CreatedBy.Should().Be("admin-1");
    }

    [Fact]
    public async Task CreateShippingAsync_WhenConcurrentCreateHitsUniqueIndex_ThrowsConflict()
    {
        var dto = ValidCreateDto();
        SetupOrder(ProcessingPaidOrder());
        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().AddAsync(It.IsAny<Shipping>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("x",
                new Exception("Cannot insert duplicate key row ... IX_Shippings_OrderId")));

        var act = () => _shippingService.CreateShippingAsync(dto, "admin-1");

        await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.ConflictException>();
    }

    [Fact]
    public async Task CreateShippingAsync_WhenOtherDbUpdateFails_DoesNotMapToConflict()
    {
        var dto = ValidCreateDto();
        SetupOrder(ProcessingPaidOrder());
        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().AddAsync(It.IsAny<Shipping>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("x", new Exception("deadlock victim")));

        var act = () => _shippingService.CreateShippingAsync(dto, "admin-1");

        await act.Should().ThrowAsync<Microsoft.EntityFrameworkCore.DbUpdateException>();
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Delivered)]
    public async Task CreateShippingAsync_RequiresProcessingOrder(OrderStatus status)
    {
        var dto = ValidCreateDto();
        var order = ProcessingPaidOrder();
        order.OrderStatus = status;
        SetupOrder(order);

        var act = () => _shippingService.CreateShippingAsync(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateShippingAsync_RequiresPaidPayment()
    {
        var dto = ValidCreateDto();
        var order = ProcessingPaidOrder();
        order.Payments.Clear();
        SetupOrder(order);

        var act = () => _shippingService.CreateShippingAsync(dto);

        await act.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task CreateShippingAsync_SecondShipmentIsRejected_UnlessEarlierOnesFailed()
    {
        var dto = ValidCreateDto();
        var order = ProcessingPaidOrder();
        order.Shippings.Add(new Shipping { Status = ShippingStatus.InTransit });
        SetupOrder(order);

        var act = () => _shippingService.CreateShippingAsync(dto);
        await act.Should().ThrowAsync<ConflictException>();

        order.Shippings.Clear();
        order.Shippings.Add(new Shipping { Status = ShippingStatus.Failed });
        await _shippingService.CreateShippingAsync(dto);
    }

    [Fact]
    public async Task CreateShippingAsync_WhenOrderMissing_ThrowsNotFound()
    {
        var dto = ValidCreateDto();
        SetupOrder(null);

        var act = () => _shippingService.CreateShippingAsync(dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    private Shipping SetupShipping(ShippingStatus status)
    {
        var shipping = new Shipping { Id = TestGuid.FromInt(5), OrderId = TestGuid.FromInt(1), Status = status };
        _unitOfWorkMock.Setup(u => u.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(It.IsAny<ShippingByIdSpecification>()))
            .ReturnsAsync(shipping);
        _mapperMock.Setup(m => m.Map<GetShippingDTO>(It.IsAny<Shipping>())).Returns(new GetShippingDTO());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        return shipping;
    }

    [Fact]
    public async Task UpdateShippingAsync_InTransit_SetsShippedDateAndHistory_OrderUntouched()
    {
        var shipping = SetupShipping(ShippingStatus.Pending);

        await _shippingService.UpdateShippingAsync(shipping.Id,
            new UpdateShippingDTO { Status = ShippingStatus.InTransit, TrackingNumber = "TRK-1" }, "admin-1");

        shipping.Status.Should().Be(ShippingStatus.InTransit);
        shipping.TrackingNumber.Should().Be("TRK-1");
        shipping.ShippedDate.Should().NotBeNull();
        shipping.StatusHistories.Should().ContainSingle().Which.ShippingStatus.Should().Be(ShippingStatus.InTransit);
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task UpdateShippingAsync_Delivered_ClosesProcessingOrderWithHistory()
    {
        var shipping = SetupShipping(ShippingStatus.InTransit);
        var order = ProcessingPaidOrder();
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetByIdAsync(shipping.OrderId)).ReturnsAsync(order);

        await _shippingService.UpdateShippingAsync(shipping.Id, new UpdateShippingDTO { Status = ShippingStatus.Delivered }, "admin-1");

        shipping.DeliveredDate.Should().NotBeNull();
        order.OrderStatus.Should().Be(OrderStatus.Delivered);
        var history = order.OrderStatusHistories.Should().ContainSingle().Subject;
        history.Source.Should().Be("Shipping");
        order.Payments.Single().Status.Should().Be(PaymentStatus.Paid);
    }

    [Theory]
    [InlineData(ShippingStatus.Pending, ShippingStatus.Delivered)]
    [InlineData(ShippingStatus.LabelCreated, ShippingStatus.Delivered)]
    [InlineData(ShippingStatus.Delivered, ShippingStatus.InTransit)]
    [InlineData(ShippingStatus.Delivered, ShippingStatus.Returned)]
    [InlineData(ShippingStatus.Failed, ShippingStatus.InTransit)]
    [InlineData(ShippingStatus.InTransit, ShippingStatus.Pending)]
    public async Task UpdateShippingAsync_InvalidTransition_IsRejected(ShippingStatus from, ShippingStatus to)
    {
        var shipping = SetupShipping(from);

        var act = () => _shippingService.UpdateShippingAsync(shipping.Id, new UpdateShippingDTO { Status = to });

        await act.Should().ThrowAsync<BadRequestException>();
        shipping.Status.Should().Be(from);
        shipping.StatusHistories.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateShippingAsync_Delivered_WhenOrderNotProcessing_IsRejectedWithoutChange()
    {
        var shipping = SetupShipping(ShippingStatus.InTransit);
        var order = ProcessingPaidOrder();
        order.OrderStatus = OrderStatus.OnHold;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetByIdAsync(shipping.OrderId)).ReturnsAsync(order);

        var act = () => _shippingService.UpdateShippingAsync(shipping.Id, new UpdateShippingDTO { Status = ShippingStatus.Delivered });

        await act.Should().ThrowAsync<ConflictException>();
        shipping.Status.Should().Be(ShippingStatus.InTransit);
    }

    [Fact]
    public async Task CreateShippingAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateShippingDTO { OrderId = Guid.Empty };
        var failures = new List<ValidationFailure> { new("OrderId", "OrderId required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _shippingService.CreateShippingAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
