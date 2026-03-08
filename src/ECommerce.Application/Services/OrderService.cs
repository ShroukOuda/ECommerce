using ECommerce.Application.DTO.Order;
using ECommerce.Core.Entities.Order;
using ECommerce.Core.Interfaces.Repositories;

namespace ECommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderDTO> _createValidator;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateOrderDTO> createValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    public async Task<IEnumerable<GetOrderDTO>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        var orders = await _unitOfWork.OrderRepository.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<GetOrderDTO>>(orders);
    }

    public async Task<GetOrderDTO> GetOrderByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _unitOfWork.OrderRepository.GetOrderWithDetailsAsync(id, ct);
        if (order is null) throw new KeyNotFoundException($"Order with ID {id} not found.");
        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task<IEnumerable<GetOrderDTO>> GetOrdersByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var orders = await _unitOfWork.OrderRepository.GetOrdersByUserIdAsync(userId, ct);
        return _mapper.Map<IEnumerable<GetOrderDTO>>(orders);
    }

    public async Task<GetOrderDTO> CreateOrderAsync(CreateOrderDTO dto, CancellationToken ct = default)
    {
        var result = await _createValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var order = new Order
        {
            UserId = dto.UserId,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            Currency = dto.Currency,
            ShippingAddressId = dto.ShippingAddressId,
            BillingAddressId = dto.BillingAddressId
        };

        await _unitOfWork.OrderRepository.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task UpdateOrderStatusAsync(UpdateOrderStatusDTO dto, CancellationToken ct = default)
    {
        var order = await _unitOfWork.OrderRepository.GetByIdAsync(dto.Id, ct);
        if (order is null) throw new KeyNotFoundException($"Order with ID {dto.Id} not found.");

        if (Enum.TryParse<ECommerce.Core.Enums.Order.OrderStatus>(dto.OrderStatus, out var status))
            order.OrderStatus = status;
        else
            throw new ArgumentException($"Invalid order status: {dto.OrderStatus}");

        await _unitOfWork.OrderRepository.UpdateAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteOrderAsync(int id, CancellationToken ct = default)
    {
        bool exists = await _unitOfWork.OrderRepository.ExistsAsync(o => o.Id == id, ct);
        if (!exists) throw new KeyNotFoundException($"Order with ID {id} not found.");
        var stub = new Order { Id = id };
        await _unitOfWork.OrderRepository.DeleteAsync(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
