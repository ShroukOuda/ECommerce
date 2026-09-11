using AutoMapper;
using FluentValidation;
using ECommerce.Application.DTO.Order;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Enums.Order;
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
            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,

                ProductVariantId =
                    itemDto.ProductVariantId,

                Quantity = itemDto.Quantity
            };

            foreach (var optionDto in itemDto.Options)
            {
                orderItem.OrderItemOptions.Add(
                    new OrderItemOption
                    {
                        OptionName =
                            optionDto.OptionName,

                        OptionValue =
                            optionDto.OptionValue,

                        PriceAdjustment =
                            optionDto.PriceAdjustment
                    });
            }

            order.OrderItems.Add(orderItem);
        }

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