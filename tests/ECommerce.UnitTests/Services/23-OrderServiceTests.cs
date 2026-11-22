using AutoMapper;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace ECommerce.UnitTests.Services;

public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateOrderDTO>> _createValidatorMock;
    private readonly IOrderService _orderService;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateOrderDTO>>();
        _orderService = new OrderService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object);
    }

    // ---------------- Customer ----------------

    [Fact]
    public async Task GetMyOrdersAsync_ShouldReturnMappedOrders()
    {
        var orders = new List<Order> { new() { Id = TestGuid.FromInt(1), UserId = "user1" } };
        var orderDtos = new List<GetOrderDTO> { new() { Id = TestGuid.FromInt(1), OrderNumber = "ORD-001" } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetAllAsync(It.IsAny<OrdersByUserSpecification>()))
            .ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetOrderDTO>>(orders))
            .Returns(orderDtos);

        var result = await _orderService.GetMyOrdersAsync("user1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMyOrderByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderByUserSpecification>()))
            .ReturnsAsync((Order?)null);

        var act = () => _orderService.GetMyOrderByIdAsync(TestGuid.FromInt(999), "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ShouldCreatePendingOrderWithHistory()
    {
        var dto = new CreateOrderDTO
        {
            Currency = "USD",
            ShippingAddressId = TestGuid.FromInt(1),
            BillingAddressId = TestGuid.FromInt(1),
            Items = new List<CreateOrderItemDTO>
            {
                new()
                {
                    ProductId = TestGuid.FromInt(1),
                    Quantity = 2,
                    Options = new List<CreateOrderItemOptionDTO>
                    {
                        new() { OptionName = "Size", OptionValue = "L", PriceAdjustment = 5m }
                    }
                }
            }
        };
        var orderDto = new GetOrderDTO { Id = TestGuid.FromInt(1) };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(orderDto);

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);

        var result = await _orderService.CreateOrderAsync("user1", dto);

        result.Should().NotBeNull();
        captured.Should().NotBeNull();
        captured!.UserId.Should().Be("user1");
        captured.OrderStatus.Should().Be(OrderStatus.Pending);
        captured.OrderNumber.Should().StartWith("ORD-");
        captured.OrderItems.Should().ContainSingle();
        captured.OrderItems.First().OrderItemOptions.Should().ContainSingle();
        captured.OrderStatusHistories.Should().ContainSingle()
            .Which.OrderStatus.Should().Be(OrderStatus.Pending);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateOrderDTO { Currency = "" };
        var failures = new List<ValidationFailure> { new("Currency", "Currency is required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenPending_ShouldCancelAndAddHistory()
    {
        var order = new Order { Id = TestGuid.FromInt(1), UserId = "user1", OrderStatus = OrderStatus.Pending };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderByUserSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _orderService.CancelOrderAsync(TestGuid.FromInt(1), "user1");

        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        order.OrderStatusHistories.Should().ContainSingle()
            .Which.OrderStatus.Should().Be(OrderStatus.Cancelled);
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().Update(order), Times.Once);
    }

    [Fact]
    public async Task CancelOrderAsync_WhenNotPending_ShouldThrowInvalidOperationException()
    {
        var order = new Order { Id = TestGuid.FromInt(1), UserId = "user1", OrderStatus = OrderStatus.Shipped };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderByUserSpecification>()))
            .ReturnsAsync(order);

        var act = () => _orderService.CancelOrderAsync(TestGuid.FromInt(1), "user1");

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderByUserSpecification>()))
            .ReturnsAsync((Order?)null);

        var act = () => _orderService.CancelOrderAsync(TestGuid.FromInt(999), "user1");

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    // ---------------- Admin ----------------

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnMappedAdminOrders()
    {
        var orders = new List<Order> { new() { Id = TestGuid.FromInt(1), OrderNumber = "ORD-001" } };
        var orderDtos = new List<GetAdminOrderDTO> { new() { Id = TestGuid.FromInt(1), OrderNumber = "ORD-001" } };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetAllAsync())
            .ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetAdminOrderDTO>>(orders))
            .Returns(orderDtos);

        var result = await _orderService.GetAllOrdersAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        var order = new Order { Id = TestGuid.FromInt(1), OrderNumber = "ORD-001" };
        var orderDto = new GetAdminOrderDTO { Id = TestGuid.FromInt(1), OrderNumber = "ORD-001" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<GetAdminOrderDTO>(order))
            .Returns(orderDto);

        var result = await _orderService.GetOrderByIdAsync(TestGuid.FromInt(1));

        result.Should().NotBeNull();
        result.OrderNumber.Should().Be("ORD-001");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync((Order?)null);

        var act = () => _orderService.GetOrderByIdAsync(TestGuid.FromInt(999));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderExists_ShouldUpdateStatusAndAddHistory()
    {
        var dto = new UpdateOrderStatusDTO { OrderStatus = "Shipped" };
        var order = new Order { Id = TestGuid.FromInt(1), OrderStatus = OrderStatus.Processing };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _orderService.UpdateOrderStatusAsync(TestGuid.FromInt(1), dto);

        order.OrderStatus.Should().Be(OrderStatus.Shipped);
        order.OrderStatusHistories.Should().ContainSingle()
            .Which.OrderStatus.Should().Be(OrderStatus.Shipped);
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().Update(order), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var dto = new UpdateOrderStatusDTO { OrderStatus = "Shipped" };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync((Order?)null);

        var act = () => _orderService.UpdateOrderStatusAsync(TestGuid.FromInt(999), dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidStatus_ShouldThrowArgumentException()
    {
        var dto = new UpdateOrderStatusDTO { OrderStatus = "InvalidStatus" };
        var order = new Order { Id = TestGuid.FromInt(1) };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);

        var act = () => _orderService.UpdateOrderStatusAsync(TestGuid.FromInt(1), dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenStatusUnchanged_ShouldNotSave()
    {
        var dto = new UpdateOrderStatusDTO { OrderStatus = "Pending" };
        var order = new Order { Id = TestGuid.FromInt(1), OrderStatus = OrderStatus.Pending };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);

        await _orderService.UpdateOrderStatusAsync(TestGuid.FromInt(1), dto);

        order.OrderStatusHistories.Should().BeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
