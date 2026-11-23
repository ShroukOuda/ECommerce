using AutoMapper;
using FluentValidation;
using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Application.Specifications.Coupons;
using ECommerce.Application.Specifications.Products;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Inventory;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Product;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderDTO> _createValidator;
    private readonly ICouponService _couponService;

    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateOrderDTO> createValidator,
        ICouponService couponService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
        _couponService = couponService;
    }

    // =========================================================
    // CUSTOMER
    // =========================================================

    public async Task<IEnumerable<GetOrderDTO>> GetMyOrdersAsync(
        string userId)
    {
        var specification =
            new OrdersByUserSpecification(userId);

        var orders =
            await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetAllAsync(specification);

        return _mapper.Map<IEnumerable<GetOrderDTO>>(orders);
    }

    public async Task<GetOrderDTO> GetMyOrderByIdAsync(
        Guid orderId,
        string userId)
    {
        var specification =
            new OrderByUserSpecification(
                orderId,
                userId);

        var order =
            await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(specification);

        if (order is null)
        {
            throw new KeyNotFoundException(
                $"Order with ID {orderId} not found.");
        }

        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task<GetOrderDTO> CreateOrderAsync(
        string userId,
        CreateOrderDTO dto)
    {
        var validationResult =
            await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors);
        }

        // Checkout converts the cart: items, quantities and
        // prices all come from the server-side cart and catalog,
        // never from client-supplied order lines.
        var cart = await _unitOfWork
            .GetRepository<Cart, Guid>()
            .GetFirstOrDefaultAsync(
                new CartDetailsSpecification(dto.CartId));

        if (cart is null || cart.UserId != userId)
        {
            throw new KeyNotFoundException(
                $"Cart with ID {dto.CartId} not found.");
        }

        if (cart.Status != CartStatus.Active ||
            cart.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Cart is no longer active.");
        }

        if (cart.CartItems.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot checkout an empty cart.");
        }

        var order = new Order
        {
            // Generated here so the coupon usage can reference
            // the order before it is saved.
            Id = Guid.NewGuid(),

            UserId = userId,

            OrderNumber =
                $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-" +
                $"{Guid.NewGuid().ToString()[..4].ToUpper()}",

            OrderType = OrderType.Standard,

            OrderStatus = OrderStatus.Pending,

            Currency = dto.Currency,

            ShippingAddressId = dto.ShippingAddressId,

            BillingAddressId = dto.BillingAddressId
        };

        var variantRepository =
            _unitOfWork.GetRepository<ProductVariant, Guid>();

        foreach (var cartItem in cart.CartItems)
        {
            // Trusted server-side pricing: catalog values only.
            var product = await _unitOfWork
                .GetRepository<Product, Guid>()
                .GetByIdAsync(cartItem.ProductId);

            if (product is null ||
                product.Status != ProductStatus.Published)
            {
                throw new ArgumentException(
                    $"Product {cartItem.ProductId} is not available.");
            }

            var variant = await variantRepository
                .GetFirstOrDefaultAsync(
                    new ProductVariantSpecification(
                        cartItem.VariantId));

            if (variant is null ||
                variant.ProductId != product.Id ||
                variant.Status != ProductVariantStatus.Active)
            {
                throw new ArgumentException(
                    $"Product variant {cartItem.VariantId} is not available for product {product.Id}.");
            }

            if (variant.StockQuantity < cartItem.Quantity)
            {
                throw new ArgumentException(
                    $"Insufficient stock for product variant {variant.Id}: requested {cartItem.Quantity}, available {variant.StockQuantity}.");
            }

            decimal unitPrice =
                (product.IsOnSale
                    ? product.SalePrice
                    : product.BasePrice)
                + variant.PriceAdjustment;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,

                ProductVariantId = cartItem.VariantId,

                ProductName = product.Name,

                VariantName = variant.VariantName,

                VariantAttributes = variant.Attributes,

                Quantity = cartItem.Quantity,

                UnitPrice = unitPrice,

                TotalPrice = unitPrice * cartItem.Quantity
            });

            // Stock is reserved at placement; cancelling a
            // pending order restocks symmetrically.
            variant.StockQuantity -= cartItem.Quantity;
            if (variant.StockQuantity == 0)
                variant.StockStatus = StockStatus.OutOfStock;
            variantRepository.Update(variant);
        }

        order.SubTotal =
            order.OrderItems.Sum(oi => oi.TotalPrice);

        // No tax or shipping rules are configured anywhere in
        // the project yet, so both default to zero until real
        // rules are provided. Placeholders, not legal/tax
        // determinations.
        order.TaxAmount = 0m;
        order.ShippingCost = 0m;

        // An applied cart discount must be verifiable end to
        // end: revalidate the recorded coupon under the current
        // coupon rules against the live cart, then record the
        // usage. An unverifiable discount rejects checkout
        // rather than silently charging the undiscounted total.
        decimal discount = 0m;

        if (cart.DiscountAmount > 0)
        {
            if (string.IsNullOrWhiteSpace(cart.CouponCode))
            {
                throw new InvalidOperationException(
                    "The cart has a discount that cannot be verified. Re-apply the coupon before checkout.");
            }

            var couponResult =
                await _couponService.ValidateCouponAsync(
                    userId,
                    new ValidateCouponDTO
                    {
                        CartId = cart.Id,
                        Code = cart.CouponCode
                    });

            var coupon = await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(
                    new CouponByCodeSpecification(cart.CouponCode));

            if (coupon is null)
            {
                throw new InvalidOperationException(
                    "The coupon applied to this cart no longer exists. Re-apply a valid coupon before checkout.");
            }

            discount = Math.Min(
                couponResult.DiscountAmount,
                order.SubTotal);

            coupon.UsedCount += 1;
            _unitOfWork
                .GetRepository<Coupon, Guid>()
                .Update(coupon);

            await _unitOfWork
                .GetRepository<CouponUsage, Guid>()
                .AddAsync(new CouponUsage
                {
                    CouponId = coupon.Id,
                    OrderId = order.Id,
                    UserId = userId,
                    DiscountAmount = discount
                });
        }

        order.TotalAmount = Math.Max(
                0m,
                order.SubTotal - discount)
            + order.TaxAmount
            + order.ShippingCost;

        // Consuming the cart prevents replaying it into a
        // second order.
        cart.Status = CartStatus.Converted;
        _unitOfWork.GetRepository<Cart, Guid>().Update(cart);

        order.OrderStatusHistories.Add(
            new OrderStatusHistory
            {
                OrderStatus = OrderStatus.Pending
            });

        await _unitOfWork
            .GetRepository<Order, Guid>()
            .AddAsync(order);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task CancelOrderAsync(
        Guid orderId,
        string userId)
    {
        var specification =
            new OrderByUserSpecification(
                orderId,
                userId);

        var order =
            await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(specification);

        if (order is null)
        {
            throw new KeyNotFoundException(
                $"Order with ID {orderId} not found.");
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be cancelled.");
        }

        order.OrderStatus = OrderStatus.Cancelled;

        order.OrderStatusHistories.Add(
            new OrderStatusHistory
            {
                OrderStatus = OrderStatus.Cancelled
            });

        await RestockOrderAsync(order);

        _unitOfWork
            .GetRepository<Order, Guid>()
            .Update(order);

        await _unitOfWork.SaveChangesAsync();
    }

    // Restores the variant stock reserved at placement. Only
    // called when an order transitions into Cancelled.
    private async Task RestockOrderAsync(Order order)
    {
        var variantRepository =
            _unitOfWork.GetRepository<ProductVariant, Guid>();

        foreach (var item in order.OrderItems)
        {
            var variant = await variantRepository
                .GetFirstOrDefaultAsync(
                    new ProductVariantSpecification(
                        item.ProductVariantId));

            if (variant is null)
                continue;

            variant.StockQuantity += item.Quantity;
            if (variant.StockStatus == StockStatus.OutOfStock &&
                variant.StockQuantity > 0)
            {
                variant.StockStatus = StockStatus.InStock;
            }

            variantRepository.Update(variant);
        }
    }

    // =========================================================
    // ADMIN
    // =========================================================

    public async Task<IEnumerable<GetAdminOrderDTO>>
        GetAllOrdersAsync()
    {
        var orders =
            await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetAllAsync();

        return _mapper.Map<IEnumerable<GetAdminOrderDTO>>(
            orders);
    }

    public async Task<GetAdminOrderDTO> GetOrderByIdAsync(
        Guid orderId)
    {
        var specification =
            new OrderDetailsSpecification(orderId);

        var order =
            await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(specification);

        if (order is null)
        {
            throw new KeyNotFoundException(
                $"Order with ID {orderId} not found.");
        }

        return _mapper.Map<GetAdminOrderDTO>(order);
    }

    public async Task<GetAdminOrderDTO>
        UpdateOrderStatusAsync(
            Guid orderId,
            UpdateOrderStatusDTO dto)
    {
        var specification =
            new OrderDetailsSpecification(orderId);

        var order =
            await _unitOfWork
                .GetRepository<Order, Guid>()
                .GetFirstOrDefaultAsync(specification);

        if (order is null)
        {
            throw new KeyNotFoundException(
                $"Order with ID {orderId} not found.");
        }

        if (!Enum.TryParse<OrderStatus>(
                dto.OrderStatus,
                true,
                out var newStatus))
        {
            throw new ArgumentException(
                $"Invalid order status: {dto.OrderStatus}");
        }

        if (order.OrderStatus == newStatus)
        {
            return _mapper.Map<GetAdminOrderDTO>(order);
        }

        if (newStatus == OrderStatus.Cancelled)
            await RestockOrderAsync(order);

        order.OrderStatus = newStatus;

        order.OrderStatusHistories.Add(
            new OrderStatusHistory
            {
                OrderStatus = newStatus
            });

        _unitOfWork
            .GetRepository<Order, Guid>()
            .Update(order);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<GetAdminOrderDTO>(order);
    }
}
