using ECommerce.Application.DTO.Order;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Orders;
using ECommerce.Domain.Enums.Order;

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

    public async Task<IEnumerable<GetOrderDTO>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync();
        return _mapper.Map<IEnumerable<GetOrderDTO>>(orders);
    }

    public async Task<GetOrderDTO> GetOrderByIdAsync(Guid id)
    {
        var spec = new OrderDetailsSpecification(id);
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(spec);
        if (order is null) throw new KeyNotFoundException($"Order with ID {id} not found.");
        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task<IEnumerable<GetOrderDTO>> GetOrdersByUserIdAsync(string userId)
    {
        var spec = new OrdersByUserSpecification(userId);
        var orders = await _unitOfWork.GetRepository<Order, Guid>().GetAllAsync(spec);
        return _mapper.Map<IEnumerable<GetOrderDTO>>(orders);
    }

    public async Task<GetOrderDTO> CreateOrderAsync(CreateOrderDTO dto)
    {
        var result = await _createValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var order = new Order
        {
            UserId = dto.UserId,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            Currency = dto.Currency,
            ShippingAddressId = dto.ShippingAddressId,
            BillingAddressId = dto.BillingAddressId
        };

        await _unitOfWork.GetRepository<Order, Guid>().AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task<GetOrderDTO> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDTO dto)
    {
        var spec = new OrderSpecification(id);
        var order = await _unitOfWork.GetRepository<Order, Guid>().GetFirstOrDefaultAsync(spec);
        if (order is null) throw new KeyNotFoundException($"Order with ID {id} not found.");
        if (Enum.TryParse<OrderStatus>(dto.OrderStatus, out var status))
            order.OrderStatus = status;
        else
            throw new ArgumentException($"Invalid order status: {dto.OrderStatus}");

        _unitOfWork.GetRepository<Order, Guid>().Update(order);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetOrderDTO>(order);
    }

    public async Task DeleteOrderAsync(Guid id)
    {
        var spec = new OrderSpecification(id);
        bool exists = await _unitOfWork.GetRepository<Order, Guid>().ExistsAsync(spec);
        if (!exists) throw new KeyNotFoundException($"Order with ID {id} not found.");
        var stub = new Order { Id = id };
        _unitOfWork.GetRepository<Order, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
    }
}
