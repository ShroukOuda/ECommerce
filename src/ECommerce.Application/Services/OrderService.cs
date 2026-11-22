using AutoMapper;
using FluentValidation;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Application.Specifications.Products;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Product;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderDTO> _createValidator;

    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateOrderDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
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

        // Checkout integrity: the cart must exist, belong to
        // the ordering user and still be active. Cart discounts
        // are intentionally NOT applied: the cart records only
        // a discount amount, not which coupon produced it, so
        // applying it here would leave no trace for refund
        // calculations. See docs/TESTING.md.
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

        var order = new Order
        {
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

        foreach (var itemDto in dto.Items)
        {
            // Trusted server-side pricing: catalog values only,
            // never client-supplied prices or adjustments.
            var product = await _unitOfWork
                .GetRepository<Product, Guid>()
                .GetByIdAsync(itemDto.ProductId);

            if (product is null ||
                product.Status != ProductStatus.Published)
            {
                throw new ArgumentException(
                    $"Product {itemDto.ProductId} is not available.");
            }

            var variant = await _unitOfWork
                .GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(
                    new ProductVariantSpecification(
                        itemDto.ProductVariantId));

            if (variant is null ||
                variant.ProductId != product.Id ||
                variant.Status != ProductVariantStatus.Active)
            {
                throw new ArgumentException(
                    $"Product variant {itemDto.ProductVariantId} is not available for product {product.Id}.");
            }

            if (variant.StockQuantity < itemDto.Quantity)
            {
                throw new ArgumentException(
                    $"Insufficient stock for product variant {variant.Id}: requested {itemDto.Quantity}, available {variant.StockQuantity}.");
            }

            decimal unitPrice =
                (product.IsOnSale
                    ? product.SalePrice
                    : product.BasePrice)
                + variant.PriceAdjustment;

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,

                ProductVariantId =
                    itemDto.ProductVariantId,

                ProductName = product.Name,

                VariantName = variant.VariantName,

                VariantAttributes = itemDto.VariantAttributes,

                Quantity = itemDto.Quantity
            };

            foreach (var optionDto in itemDto.Options)
            {
                var option = await _unitOfWork
                    .GetRepository<ProductOption, Guid>()
                    .GetFirstOrDefaultAsync(
                        new ProductOptionSpecification(
                            optionDto.ProductOptionId));

                if (option is null ||
                    option.ProductId != product.Id)
                {
                    throw new ArgumentException(
                        $"Product option {optionDto.ProductOptionId} is not available for product {product.Id}.");
                }

                // Free-text options (no defined values) use the
                // option's flat catalog price; options with
                // defined values price from the matched value.
                decimal optionPrice = option.PriceValue;

                if (option.ProductOptionValues.Count > 0)
                {
                    var optionValue = option.ProductOptionValues
                        .FirstOrDefault(v =>
                            string.Equals(
                                v.Value,
                                optionDto.OptionValue,
                                StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(
                                v.Label,
                                optionDto.OptionValue,
                                StringComparison.OrdinalIgnoreCase));

                    if (optionValue is null)
                    {
                        throw new ArgumentException(
                            $"Option value '{optionDto.OptionValue}' is not available for option {option.Id}.");
                    }

                    optionPrice = optionValue.PriceValue;
                }

                orderItem.OrderItemOptions.Add(
                    new OrderItemOption
                    {
                        OptionName = option.Name,

                        OptionValue = optionDto.OptionValue,

                        PriceAdjustment = optionPrice
                    });

                unitPrice += optionPrice;
            }

            orderItem.UnitPrice = unitPrice;
            orderItem.TotalPrice =
                unitPrice * itemDto.Quantity;

            order.OrderItems.Add(orderItem);
        }

        order.SubTotal =
            order.OrderItems.Sum(oi => oi.TotalPrice);

        // No tax or shipping rules are configured anywhere in
        // the project yet, so both default to zero until real
        // rules are provided. Placeholders, not legal/tax
        // determinations.
        order.TaxAmount = 0m;
        order.ShippingCost = 0m;
        order.TotalAmount = order.SubTotal
            + order.TaxAmount
            + order.ShippingCost;

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

        _unitOfWork
            .GetRepository<Order, Guid>()
            .Update(order);

        await _unitOfWork.SaveChangesAsync();
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
