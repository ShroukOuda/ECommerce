using AutoMapper;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Core.Entities.Order;
using ECommerce.Core.Interfaces.Repositories;
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

    [Fact]
    public async Task GetAllOrdersAsync_ShouldReturnMappedOrders()
    {
        var orders = new List<Order> { new() { Id = 1, OrderNumber = "ORD-001" } };
        var orderDtos = new List<GetOrderDTO> { new() { Id = 1, OrderNumber = "ORD-001" } };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetOrderDTO>>(orders))
            .Returns(orderDtos);

        var result = await _orderService.GetAllOrdersAsync();

        result.Should().HaveCount(1);
        result.First().OrderNumber.Should().Be("ORD-001");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnOrder()
    {
        var order = new Order { Id = 1, OrderNumber = "ORD-001" };
        var orderDto = new GetOrderDTO { Id = 1, OrderNumber = "ORD-001" };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderWithDetailsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(order))
            .Returns(orderDto);

        var result = await _orderService.GetOrderByIdAsync(1);

        result.Should().NotBeNull();
        result.OrderNumber.Should().Be("ORD-001");
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrderWithDetailsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var act = () => _orderService.GetOrderByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetOrdersByUserIdAsync_ShouldReturnMappedOrders()
    {
        var orders = new List<Order> { new() { Id = 1, UserId = "user1" } };
        var orderDtos = new List<GetOrderDTO> { new() { Id = 1, UserId = "user1" } };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetOrdersByUserIdAsync("user1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        _mapperMock.Setup(m => m.Map<IEnumerable<GetOrderDTO>>(orders))
            .Returns(orderDtos);

        var result = await _orderService.GetOrdersByUserIdAsync("user1");

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ShouldCreateOrder()
    {
        var dto = new CreateOrderDTO
        {
            UserId = "user1",
            Currency = "USD",
            ShippingAddressId = 1,
            BillingAddressId = 1,
            Items = new List<CreateOrderItemDTO> { new() { ProductId = 1, Quantity = 2 } }
        };
        var orderDto = new GetOrderDTO { Id = 1 };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.OrderRepository.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(orderDto);

        var result = await _orderService.CreateOrderAsync(dto);

        result.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.OrderRepository.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var dto = new CreateOrderDTO { UserId = "" };
        var failures = new List<ValidationFailure> { new("UserId", "User ID is required") };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var act = () => _orderService.CreateOrderAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenOrderExists_ShouldUpdateStatus()
    {
        var dto = new UpdateOrderStatusDTO { Id = 1, OrderStatus = "Shipped" };
        var order = new Order { Id = 1 };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _orderService.UpdateOrderStatusAsync(dto);

        _unitOfWorkMock.Verify(u => u.OrderRepository.UpdateAsync(order, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        var dto = new UpdateOrderStatusDTO { Id = 999, OrderStatus = "Shipped" };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var act = () => _orderService.UpdateOrderStatusAsync(dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidStatus_ShouldThrowArgumentException()
    {
        var dto = new UpdateOrderStatusDTO { Id = 1, OrderStatus = "InvalidStatus" };
        var order = new Order { Id = 1 };

        _unitOfWorkMock.Setup(u => u.OrderRepository.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var act = () => _orderService.UpdateOrderStatusAsync(dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task DeleteOrderAsync_WhenOrderExists_ShouldDeleteOrder()
    {
        _unitOfWorkMock.Setup(u => u.OrderRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        await _orderService.DeleteOrderAsync(1);

        _unitOfWorkMock.Verify(u => u.OrderRepository.DeleteAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOrderAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
    {
        _unitOfWorkMock.Setup(u => u.OrderRepository.ExistsAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Order, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _orderService.DeleteOrderAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
