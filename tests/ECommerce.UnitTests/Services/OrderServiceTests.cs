using AutoMapper;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.DTO.Coupon;
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
    private readonly Mock<ICouponService> _couponServiceMock;
    private readonly IOrderService _orderService;
    private readonly Mock<ICheckoutCurrencyProvider> _currencyProviderMock;

    public OrderServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _createValidatorMock = new Mock<IValidator<CreateOrderDTO>>();
        _couponServiceMock = new Mock<ICouponService>();
        _currencyProviderMock = new Mock<ICheckoutCurrencyProvider>();
        _currencyProviderMock.SetupGet(x => x.Currency).Returns("USD");
        _orderService = new OrderService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _createValidatorMock.Object,
            _couponServiceMock.Object,
            _currencyProviderMock.Object);

        _unitOfWorkMock.Setup(u => u.GetRepository<ECommerce.Domain.Entities.Users.Address, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new ECommerce.Domain.Entities.Users.Address { Id = id, UserId = "user1" });
        // Default checkout fixtures: an owned active cart with
        // one priced item, a published product, an active
        // in-stock variant. Individual tests override these.
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(DefaultCart());
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>()
                .GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(DefaultProduct());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(DefaultVariant());
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CouponByCodeSpecification>()))
            .ReturnsAsync(DefaultCoupon());
    }

    internal static Cart DefaultCart() => new()
    {
        Id = TestGuid.FromInt(100),
        UserId = "user1",
        Status = CartStatus.Active,
        ExpiresAt = DateTime.UtcNow.AddDays(1),
        CartItems = new List<CartItem>
        {
            new()
            {
                Id = TestGuid.FromInt(101),
                ProductId = TestGuid.FromInt(1),
                VariantId = TestGuid.FromInt(2),
                Quantity = 2,
                Price = 100m
            }
        }
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

    internal static Coupon DefaultCoupon() => new()
    {
        Id = TestGuid.FromInt(20),
        Code = "SAVE10"
    };

    internal static CreateOrderDTO DefaultCreateDto() => new()
    {
        CartId = TestGuid.FromInt(100),
        ShippingAddressId = TestGuid.FromInt(50),
        BillingAddressId = TestGuid.FromInt(50)
    };

    private void SetupValidCreate(Order? captured)
    {
    }

    private void SetupCreateOrder(Order? _)
    {
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

    [Theory]
    [InlineData("USD")]
    [InlineData("EGP")]
    [InlineData("EUR")]
    public async Task CreateOrderAsync_WithValidData_ShouldCreatePricedOrderFromCartAndConsumeIt(string serverCurrency)
    {
        var dto = DefaultCreateDto();
        _currencyProviderMock.SetupGet(x => x.Currency).Returns(serverCurrency);
        var cart = DefaultCart();
        var variant = DefaultVariant();

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(new GetOrderDTO { Id = TestGuid.FromInt(1) });

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);

        var result = await _orderService.CreateOrderAsync("user1", dto);

        result.Should().NotBeNull();
        captured.Should().NotBeNull();
        captured!.UserId.Should().Be("user1");
        captured.Currency.Should().Be(serverCurrency);
        _currencyProviderMock.VerifyGet(x => x.Currency, Times.Once);
        captured.OrderStatus.Should().Be(OrderStatus.Pending);
        captured.OrderNumber.Should().StartWith("ORD-");
        var line = captured.OrderItems.Should().ContainSingle().Subject;
        line.UnitPrice.Should().Be(100m);
        line.TotalPrice.Should().Be(200m);
        line.ProductName.Should().Be("Test Product");
        line.VariantName.Should().Be("Standard");
        captured.SubTotal.Should().Be(200m);
        captured.TaxAmount.Should().Be(0m);
        captured.ShippingCost.Should().Be(0m);
        captured.TotalAmount.Should().Be(200m);
        captured.OrderStatusHistories.Should().ContainSingle()
            .Which.OrderStatus.Should().Be(OrderStatus.Pending);

        // Stock reserved at placement
        variant.StockQuantity.Should().Be(8);
        _unitOfWorkMock.Verify(u => u.GetRepository<ProductVariant, Guid>().Update(variant), Times.Once);

        // Cart consumed to prevent replay
        cart.Status.Should().Be(CartStatus.Converted);
        _unitOfWorkMock.Verify(u => u.GetRepository<Cart, Guid>().Update(cart), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenProductOnSale_ShouldUseSalePricePlusVariantAdjustment()
    {
        var dto = DefaultCreateDto();
        var product = DefaultProduct();
        product.BasePrice = 100m;
        product.SalePrice = 80m; // on sale
        var variant = DefaultVariant();
        variant.PriceAdjustment = 10m;

        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(new GetOrderDTO());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);

        await _orderService.CreateOrderAsync("user1", dto);

        // 80 sale + 10 variant = 90 per unit, qty 2
        captured!.OrderItems.First().UnitPrice.Should().Be(90m);
        captured.OrderItems.First().TotalPrice.Should().Be(180m);
        captured.TotalAmount.Should().Be(180m);
    }

    [Fact]
    public async Task CreateOrderAsync_WithVerifiableDiscount_ShouldApplyItAndRecordUsage()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.DiscountAmount = 20m;
        cart.CouponCode = "SAVE10";
        var coupon = DefaultCoupon();

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CouponByCodeSpecification>()))
            .ReturnsAsync(coupon);
        _couponServiceMock.Setup(c => c.ValidateCouponAsync("user1", It.IsAny<ValidateCouponDTO>()))
            .ReturnsAsync(new CouponResultDTO { Code = "SAVE10", DiscountAmount = 20m });
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(new GetOrderDTO());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);
        CouponUsage? usage = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<CouponUsage, Guid>().AddAsync(It.IsAny<CouponUsage>()))
            .Callback<CouponUsage>(u => usage = u)
            .Returns(Task.CompletedTask);

        await _orderService.CreateOrderAsync("user1", dto);

        captured!.TotalAmount.Should().Be(180m); // 200 - 20
        usage.Should().NotBeNull();
        usage!.CouponId.Should().Be(coupon.Id);
        usage.OrderId.Should().Be(captured.Id);
        usage.UserId.Should().Be("user1");
        usage.DiscountAmount.Should().Be(20m);
        coupon.UsedCount.Should().Be(1);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenDiscountExceedsSubTotal_ShouldClampTotalAtZero()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.DiscountAmount = 999m;
        cart.CouponCode = "SAVE10";

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _couponServiceMock.Setup(c => c.ValidateCouponAsync("user1", It.IsAny<ValidateCouponDTO>()))
            .ReturnsAsync(new CouponResultDTO { Code = "SAVE10", DiscountAmount = 999m });
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mapperMock.Setup(m => m.Map<GetOrderDTO>(It.IsAny<Order>())).Returns(new GetOrderDTO());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()))
            .Callback<Order>(o => captured = o)
            .Returns(Task.CompletedTask);
        CouponUsage? usage = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<CouponUsage, Guid>().AddAsync(It.IsAny<CouponUsage>()))
            .Callback<CouponUsage>(u => usage = u)
            .Returns(Task.CompletedTask);

        await _orderService.CreateOrderAsync("user1", dto);

        captured!.TotalAmount.Should().Be(0m);
        usage!.DiscountAmount.Should().Be(200m); // clamped to subtotal
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartDiscountCannotBeVerified_ShouldRejectCheckout()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.DiscountAmount = 20m;
        cart.CouponCode = null; // no trace of which coupon

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenAppliedCouponNoLongerExists_ShouldRejectCheckout()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.DiscountAmount = 20m;
        cart.CouponCode = "GONE";

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _couponServiceMock.Setup(c => c.ValidateCouponAsync("user1", It.IsAny<ValidateCouponDTO>()))
            .ReturnsAsync(new CouponResultDTO { Code = "GONE", DiscountAmount = 20m });
        _unitOfWorkMock.Setup(u => u.GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CouponByCodeSpecification>()))
            .ReturnsAsync((Coupon?)null);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCouponRevalidationFails_ShouldRejectCheckout()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.DiscountAmount = 20m;
        cart.CouponCode = "SAVE10";

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _couponServiceMock.Setup(c => c.ValidateCouponAsync("user1", It.IsAny<ValidateCouponDTO>()))
            .ThrowsAsync(new InvalidOperationException("This coupon has expired."));
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartEmpty_ShouldThrowInvalidOperationException()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.CartItems.Clear();

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartAlreadyConverted_ShouldRejectReplay()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.Status = CartStatus.Converted;

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
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
        variant.StockQuantity = 1; // cart requests 2

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ArgumentException>();
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrderAsync_WhenCartNotOwnedByUser_ShouldThrowKeyNotFoundException()
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart();
        cart.UserId = "someone-else";

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

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

        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>()))
            .ReturnsAsync(cart);
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateOrderThenReturn_ShouldRefundProRataOfPersistedPaidValue()
    {
        // End-to-end create-to-return regression at service
        // level: order prices persisted by checkout are what
        // the refund calculation uses.
        var dto = DefaultCreateDto();

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
        var dto = new CreateOrderDTO();

        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("CartId", "required") }));

        var act = () => _orderService.CreateOrderAsync("user1", dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenPending_ShouldCancelAddHistoryAndRestock()
    {
        var variant = DefaultVariant();
        variant.StockQuantity = 8; // 2 were reserved at checkout
        var order = new Order
        {
            Id = TestGuid.FromInt(1),
            UserId = "user1",
            OrderStatus = OrderStatus.Pending,
            OrderItems = new List<OrderItem>
            {
                new() { ProductVariantId = variant.Id, Quantity = 2 }
            }
        };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderByUserSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _orderService.CancelOrderAsync(TestGuid.FromInt(1), "user1");

        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        order.OrderStatusHistories.Should().ContainSingle()
            .Which.OrderStatus.Should().Be(OrderStatus.Cancelled);
        variant.StockQuantity.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.GetRepository<ProductVariant, Guid>().Update(variant), Times.Once);
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

    [Fact]
    public async Task UpdateOrderStatusAsync_ToCancelled_ShouldRestock()
    {
        var dto = new UpdateOrderStatusDTO { OrderStatus = "Cancelled" };
        var variant = DefaultVariant();
        variant.StockQuantity = 8;
        var order = new Order
        {
            Id = TestGuid.FromInt(1),
            OrderStatus = OrderStatus.Processing,
            OrderItems = new List<OrderItem>
            {
                new() { ProductVariantId = variant.Id, Quantity = 2 }
            }
        };

        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(It.IsAny<ProductVariantSpecification>()))
            .ReturnsAsync(variant);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<GetAdminOrderDTO>(It.IsAny<Order>())).Returns(new GetAdminOrderDTO());

        await _orderService.UpdateOrderStatusAsync(TestGuid.FromInt(1), dto);

        order.OrderStatus.Should().Be(OrderStatus.Cancelled);
        variant.StockQuantity.Should().Be(10);
        _unitOfWorkMock.Verify(u => u.GetRepository<ProductVariant, Guid>().Update(variant), Times.Once);
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.OnHold)]
    public async Task AdminCancel_WhenAnyAttemptPaid_RequiresReviewBeforeAnyMutation(OrderStatus status)
    {
        var variant = DefaultVariant();
        variant.StockQuantity = 8;
        var order = new Order
        {
            Id = TestGuid.FromInt(1), OrderStatus = status,
            OrderItems = new List<OrderItem> { new() { ProductVariantId = variant.Id, Quantity = 2 } },
            Payments = new List<ECommerce.Domain.Entities.Payments.Payment>
            {
                new() { Status = ECommerce.Domain.Enums.Payment.PaymentStatus.Paid },
                new() { Status = ECommerce.Domain.Enums.Payment.PaymentStatus.Failed, UpdatedAt = DateTime.UtcNow }
            }
        };
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>()))
            .ReturnsAsync(order);

        // Retrying cannot cancel, restock, mutate payment or create history.
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var act = () => _orderService.UpdateOrderStatusAsync(order.Id, new() { OrderStatus = "Cancelled" });
            await act.Should().ThrowAsync<ECommerce.Domain.Exceptions.ConflictException>().WithMessage("*manual review*");
        }
        order.OrderStatus.Should().Be(status);
        order.OrderStatusHistories.Should().BeEmpty();
        variant.StockQuantity.Should().Be(8);
        order.Payments.First().Status.Should().Be(ECommerce.Domain.Enums.Payment.PaymentStatus.Paid);
        _unitOfWorkMock.Verify(u => u.GetRepository<ProductVariant, Guid>(), Times.Never);
        _unitOfWorkMock.Verify(u => u.GetRepository<Order, Guid>().Update(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AdminCancel_AlreadyCancelledPaidOrder_RemainsNoOp()
    {
        var order = new Order { Id = TestGuid.FromInt(1), OrderStatus = OrderStatus.Cancelled,
            Payments = new List<ECommerce.Domain.Entities.Payments.Payment> { new() { Status = ECommerce.Domain.Enums.Payment.PaymentStatus.Paid } } };
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderDetailsSpecification>())).ReturnsAsync(order);
        await _orderService.UpdateOrderStatusAsync(order.Id, new() { OrderStatus = "Cancelled" });
        order.OrderStatusHistories.Should().BeEmpty();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        _unitOfWorkMock.Verify(u => u.GetRepository<ProductVariant, Guid>(), Times.Never);
    }

    [Theory]
    [InlineData(false, 5)]
    [InlineData(true, 5)]
    [InlineData(false, 1)]
    public async Task SimpleProduct_ReservesProductStock_OrRejectsUnsafeOmission(bool hasVariants, int stock)
    {
        var dto = DefaultCreateDto();
        var cart = DefaultCart(); cart.CartItems.First().VariantId = null;
        var product = DefaultProduct(); product.StockQuantity = stock;
        _createValidatorMock.Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _unitOfWorkMock.Setup(u => u.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(It.IsAny<CartDetailsSpecification>())).ReturnsAsync(cart);
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.GetRepository<ProductVariant, Guid>().ExistsAsync(It.IsAny<ProductVariantsByProductSpecification>())).ReturnsAsync(hasVariants);
        Order? captured = null;
        _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().AddAsync(It.IsAny<Order>())).Callback<Order>(o => captured = o).Returns(Task.CompletedTask);
        if (hasVariants || stock < 2)
        {
            await FluentActions.Invoking(() => _orderService.CreateOrderAsync("user1", dto)).Should().ThrowAsync<Exception>();
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }
        else
        {
            await _orderService.CreateOrderAsync("user1", dto);
            captured!.OrderItems.Should().ContainSingle().Which.ProductVariantId.Should().BeNull();
            product.StockQuantity.Should().Be(3);
            _unitOfWorkMock.Verify(u => u.GetRepository<Product, Guid>().Update(product), Times.Once);
            // Cancel must restore the original product stock even if variants are added later.
            _unitOfWorkMock.Setup(u => u.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(It.IsAny<OrderByUserSpecification>())).ReturnsAsync(captured);
            await _orderService.CancelOrderAsync(captured.Id, "user1");
            product.StockQuantity.Should().Be(5);
        }
    }
}
