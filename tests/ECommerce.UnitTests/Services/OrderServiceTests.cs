using AutoMapper;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.DTO.Return;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Application.Specifications.Coupons;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Application.Specifications.Products;
using ECommerce.Application.Specifications.Returns;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Entities.Returns;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Product;
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

        // Default checkout fixtures: an owned active cart, a
        // published product, an active in-stock variant and a
        // free-text option. Individual tests override these.
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(DefaultCart());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()
                .GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(DefaultProduct());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(DefaultVariant());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductOption, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductOptionSpecification>()))
            .ReturnsAsync(DefaultOption());
    }

    internal static Cart DefaultCart() => new()
    {
        Id = TestGuid.FromInt(100),
        UserId = "user1",
        Status = CartStatus.Active,
        ExpiresAt = DateTime.UtcNow.AddDays(1)
    };

    internal static Product DefaultProduct() => new()
    {
        Id = TestGuid.FromInt(1),
        Name = "Test Product",
        Status = ProductStatus.Published,
        BasePrice = 100m,
        SalePrice = 0m
    };

    internal static ProductVariant DefaultVariant() => new()
    {
        Id = TestGuid.FromInt(2),
        ProductId = TestGuid.FromInt(1),
        VariantName = "Standard",
        Status = ProductVariantStatus.Active,
        StockQuantity = 10,
        PriceAdjustment = 0m
    };

    internal static ProductOption DefaultOption() => new()
    {
        Id = TestGuid.FromInt(3),
        ProductId = TestGuid.FromInt(1),
        Name = "Size",
        PriceValue = 5m
    };

    internal static CreateOrderDTO DefaultCreateDto() => new()
    {
        CartId = TestGuid.FromInt(100),
        Currency = "USD",
        ShippingAddressId = TestGuid.FromInt(50),
        BillingAddressId = TestGuid.FromInt(50),
        Items = new List<CreateOrderItemDTO>
        {
            new()
            {
                ProductId = TestGuid.FromInt(1),
                ProductVariantId = TestGuid.FromInt(2),
                Quantity = 2,
                Options = new List<CreateOrderItemOptionDTO>
                {
                    new() { ProductOptionId = TestGuid.FromInt(3), OptionValue = "L", PriceAdjustment = 999m }
                }
            }
        }
    };

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
        var dto = DefaultCreateDto();
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
        // 100 base + 0 variant + 5 option = 105 per unit, qty 2
        captured.OrderItems.First().UnitPrice.Should().Be(105m);
        captured.OrderItems.First().TotalPrice.Should().Be(210m);
        captured.SubTotal.Should().Be(210m);
        captured.TotalAmount.Should().Be(210m);
        // Client-supplied option price (999) must be ignored
        captured.OrderItems.First().OrderItemOptions.First()
            .PriceAdjustment.Should().Be(5m);
        captured.OrderStatusHistories.Should().ContainSingle()
            .Which.OrderStatus.Should().Be(OrderStatus.Pending);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenProductOnSale_ShouldUseSalePricePlusVariantAndOptionValue()
    {
        var dto = DefaultCreateDto();
        var product = DefaultProduct();
        product.BasePrice = 100m;
        product.SalePrice = 80m; // on sale
        var variant = DefaultVariant();
        variant.PriceAdjustment = 10m;
        var option = DefaultOption();
        option.PriceValue = 99m; // ignored: option has values
        option.ProductOptionValues = new List<ProductOptionValue>
        {
            new() { Value = "L", Label = "Large", PriceValue = 5m }
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductOption, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductOptionSpecification>()))
            .ReturnsAsync(option);
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(new GetOrderDTO());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);

        await _orderService.CreateOrderAsync("user1", dto);

        // 80 sale + 10 variant + 5 option value = 95 per unit
        captured!.OrderItems.First().UnitPrice.Should().Be(95m);
        captured.OrderItems.First().TotalPrice.Should().Be(190m);
        captured.SubTotal.Should().Be(190m);
        captured.TotalAmount.Should().Be(190m);
        captured.OrderItems.First().ProductName.Should().Be("Test Product");
        captured.OrderItems.First().VariantName.Should().Be("Standard");
    }

    [Fact]
    public async Task CreateOrderAsync_WhenProductNotPublished_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();
        var product = DefaultProduct();
        product.Status = ProductStatus.Draft;

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenProductMissing_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product?)null);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenVariantBelongsToAnotherProduct_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();
        var variant = DefaultVariant();
        variant.ProductId = TestGuid.FromInt(999);

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenVariantInactive_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();
        var variant = DefaultVariant();
        variant.Status = ProductVariantStatus.Inactive;

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenStockInsufficient_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();
        var variant = DefaultVariant();
        variant.StockQuantity = 1; // dto requests 2

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartNotOwnedByUser_ShouldThrowKeyNotFoundException()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.UserId = "someone-else";

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartExpired_ShouldThrowInvalidOperationException()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenOptionBelongsToAnotherProduct_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();
        var option = DefaultOption();
        option.ProductId = TestGuid.FromInt(999);

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductOption, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductOptionSpecification>()))
            .ReturnsAsync(option);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenOptionValueNotInCatalog_ShouldThrowArgumentException()
    {
        var dto = DefaultCreateDto();
        var option = DefaultOption();
        option.ProductOptionValues = new List<ProductOptionValue>
        {
            new() { Value = "M", Label = "Medium", PriceValue = 0m }
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductOption, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductOptionSpecification>()))
            .ReturnsAsync(option);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateOrderThenReturn_ShouldRefundProRataOfPersistedPaidValue()
    {
        // End-to-end create-to-return regression at service
        // level: order prices persisted by checkout are what
        // the refund calculation uses.
        var dto = DefaultCreateDto();
        dto.Items[0].Options.Clear(); // 100 base, qty 2

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(new GetOrderDTO());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);

        await _orderService.CreateOrderAsync("user1", dto);

        captured.Should().NotBeNull();
        captured!.OrderItems.First().Id = TestGuid.FromInt(500); // simulate persistence

        var returnUowMock = new Mock<IUnitOfWork>();
        var returnMapperMock = new Mock<IMapper>();
        var returnValidatorMock = new Mock<IValidator<CreateReturnRequestDTO>>();
        var returnService = new ReturnService(
            returnUowMock.Object,
            returnMapperMock.Object,
            returnValidatorMock.Object);

        var returnDto = new CreateReturnRequestDTO
        {
            OrderId = captured.Id,
            Items = new List<CreateReturnItemDTO>
            {
                new() { OrderItemId = TestGuid.FromInt(500), Quantity = 1, Reason = "Broken" }
            }
        };
        var mappedReturn = new ReturnRequest { OrderId = captured.Id };

        returnValidatorMock.Setup(v => v.ValidateAsync(returnDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        returnUowMock.Setup(u => u.GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(captured);
        returnUowMock.Setup(u => u.GetRepository<ReturnItem, Guid>()
                .GetAllAsync(It.IsAny<ReturnItemsByOrderSpecification>()))
            .ReturnsAsync(new List<ReturnItem>());
        returnUowMock.Setup(u => u.GetRepository<CouponUsage, Guid>()
                .GetAllAsync(It.IsAny<CouponUsagesByOrderSpecification>()))
            .ReturnsAsync(new List<CouponUsage>());
        returnMapperMock.Setup(m => m.Map<ReturnRequest>(returnDto)).Returns(mappedReturn);
        returnUowMock.Setup(u => u.GetRepository<ReturnRequest, Guid>().AddAsync(It.IsAny<ReturnRequest>()))
            .Returns(Task.CompletedTask);
        returnUowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await returnService.CreateReturnAsync("user1", returnDto);

        // Paid 100 per unit; returning 1 of 2 refunds 100.
        mappedReturn.RefundAmount.Should().Be(100m);
        mappedReturn.ReturnItems.Should().ContainSingle()
            .Which.RefundAmount.Should().Be(100m);
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
