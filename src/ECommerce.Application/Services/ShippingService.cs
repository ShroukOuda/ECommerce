using ECommerce.Application.DTO.Shipping;
using ECommerce.Domain.Entities.Shippings;
using ECommerce.Domain.Enums.Shipping;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Shippings;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Payment;

namespace ECommerce.Application.Services;

public class ShippingService : IShippingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateShippingDTO> _createValidator;

    public ShippingService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateShippingDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetShippingDTO>> GetMyShippingsAsync(string userId)
    {
        var spec = new ShippingsByUserSpecification(userId);
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<GetShippingDTO> GetMyShippingByIdAsync(Guid shippingId, string userId)
    {
        var spec = new ShippingsByUserSpecification(shippingId, userId);
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(spec);
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {shippingId} not found for user {userId}.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<GetShippingDTO> GetMyShippingByOrderIdAsync(Guid orderId, string userId)
    {
        var spec = new ShippingsByOrderSpecification(orderId, userId);
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(spec);
        if (shipping is null) throw new KeyNotFoundException($"Shipping for order ID {orderId} not found for user {userId}.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<IEnumerable<GetShippingDTO>> GetAllShippingsAsync()
    {
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync();
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<IEnumerable<GetShippingDTO>> GetShippingsByOrderIdAsync(Guid orderId)
    {
        var spec = new ShippingsByOrderSpecification(orderId);
        var shippings = await _unitOfWork.GetRepository<Shipping, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetShippingDTO>>(shippings);
    }

    public async Task<GetShippingDTO> GetShippingByIdAsync(Guid id)
    {
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(new ShippingByIdSpecification(id));
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {id} not found.");
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    // The whole order ships together: one shipment per order. A new one is only
    // allowed after earlier attempts Failed. Shipping.Status owns the parcel;
    // the order only gets closed (Delivered) as a consequence of delivery.
    public async Task<GetShippingDTO> CreateShippingAsync(CreateShippingDTO dto, string? changedBy = null)
    {
        var result = await _createValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var order = await _unitOfWork.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(new OrderDetailsSpecification(dto.OrderId));
        if (order is null) throw new KeyNotFoundException($"Order with ID {dto.OrderId} not found.");

        if (order.OrderStatus != OrderStatus.Processing)
            throw new BadRequestException("A shipment can only be created while the order is Processing.");
        if (!order.Payments.Any(p => p.Status == PaymentStatus.Paid))
            throw new BadRequestException("A shipment cannot be created before the order is paid.");
        if (order.Shippings.Any(sh => sh.Status != ShippingStatus.Failed))
            throw new ConflictException("This order already has a shipment. The whole order ships together.");
        if (order.ShippingAddressId.HasValue && order.ShippingAddressId.Value != dto.AddressId)
            throw new BadRequestException("The shipment address must be the order's shipping address.");
        if (dto.Status != ShippingStatus.Pending)
            throw new BadRequestException("A new shipment starts as Pending.");

        var shipping = _mapper.Map<Shipping>(dto);
        shipping.Status = ShippingStatus.Pending;
        shipping.StatusHistories.Add(NewHistory(ShippingStatus.Pending, changedBy, "Shipment created"));

        await _unitOfWork.GetRepository<Shipping, Guid>().AddAsync(shipping);
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            when (IsActiveShipmentUniqueViolation(ex))
        {
            throw new ConflictException("This order already has a shipment. The whole order ships together.");
        }
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    public async Task<GetShippingDTO> UpdateShippingAsync(Guid shippingId, UpdateShippingDTO dto, string? changedBy = null)
    {
        var shipping = await _unitOfWork.GetRepository<Shipping, Guid>().GetFirstOrDefaultAsync(new ShippingByIdSpecification(shippingId));
        if (shipping is null) throw new KeyNotFoundException($"Shipping with ID {shippingId} not found.");
        if (!Enum.IsDefined(dto.Status)) throw new BadRequestException("Invalid shipping status.");
        if (dto.Method != 0 && !Enum.IsDefined(dto.Method)) throw new BadRequestException("Invalid shipping method.");
        if (dto.Cost is < 0) throw new BadRequestException("Shipping cost cannot be negative.");

        var statusChanged = dto.Status != shipping.Status;
        Order? order = null;
        if (statusChanged)
        {
            var allowed = shipping.Status switch
            {
                ShippingStatus.Pending => dto.Status is ShippingStatus.LabelCreated or ShippingStatus.InTransit,
                ShippingStatus.LabelCreated => dto.Status is ShippingStatus.InTransit,
                ShippingStatus.InTransit => dto.Status is ShippingStatus.OutForDelivery or ShippingStatus.Delivered or ShippingStatus.Failed,
                ShippingStatus.OutForDelivery => dto.Status is ShippingStatus.Delivered or ShippingStatus.Failed,
                // Delivered, Failed and Returned are final. Re-delivery and return handling are not defined yet.
                _ => false
            };
            if (!allowed)
                throw new BadRequestException($"A shipment cannot change from {shipping.Status} to {dto.Status}.");

            if (dto.Status == ShippingStatus.Delivered)
            {
                order = await _unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(shipping.OrderId);
                if (order is null || order.OrderStatus != OrderStatus.Processing)
                    throw new ConflictException("The order must be Processing to complete delivery.");
            }
        }
        else if (shipping.Status is ShippingStatus.Delivered or ShippingStatus.Failed or ShippingStatus.Returned)
        {
            throw new BadRequestException($"A {shipping.Status} shipment can no longer be edited.");
        }

        if (dto.TrackingNumber is not null) shipping.TrackingNumber = dto.TrackingNumber;
        if (dto.Cost.HasValue) shipping.Cost = dto.Cost.Value;
        if (dto.Method != 0) shipping.Method = dto.Method;

        if (statusChanged)
        {
            var now = DateTime.UtcNow;
            shipping.Status = dto.Status;
            if (dto.Status == ShippingStatus.InTransit) shipping.ShippedDate ??= now;
            if (dto.Status == ShippingStatus.OutForDelivery) shipping.ShippedDate ??= now;
            if (dto.Status == ShippingStatus.Delivered)
            {
                shipping.ShippedDate ??= now;
                shipping.DeliveredDate = now;
            }
            shipping.StatusHistories.Add(NewHistory(dto.Status, changedBy, null));

            if (order is not null)
            {
                order.OrderStatus = OrderStatus.Delivered;
                order.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderStatus = OrderStatus.Delivered,
                    Source = "Shipping",
                    CreatedBy = changedBy,
                    Reason = "Shipment delivered"
                });
                _unitOfWork.GetRepository<Order, Guid>().Update(order);
            }
        }

        _unitOfWork.GetRepository<Shipping, Guid>().Update(shipping);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetShippingDTO>(shipping);
    }

    // Only the filtered unique index on Shippings.OrderId counts as a
    // concurrent-create conflict. SQL Server reports unique index
    // violations as error 2601 and constraint violations as 2627, and
    // names the index in the message. Any other DbUpdateException is
    // rethrown untouched.
    private static bool IsActiveShipmentUniqueViolation(Microsoft.EntityFrameworkCore.DbUpdateException ex)
    {
        var inner = ex.InnerException;
        if (inner is null) return false;
        var number = inner.GetType().GetProperty("Number")?.GetValue(inner) as int?;
        var uniqueError = number is 2601 or 2627
            || inner.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase);
        return uniqueError && inner.Message.Contains("IX_Shippings_OrderId", StringComparison.OrdinalIgnoreCase);
    }

    private static ShippingStatusHistory NewHistory(ShippingStatus status, string? changedBy, string? reason) => new()
    {
        ShippingStatus = status,
        Source = changedBy is null ? "System" : "Admin",
        CreatedBy = changedBy,
        Reason = reason
    };
}
