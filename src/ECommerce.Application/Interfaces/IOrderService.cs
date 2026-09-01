using ECommerce.Application.DTO.Order;

namespace ECommerce.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<GetOrderDTO>> GetAllOrdersAsync(CancellationToken ct = default);
    Task<GetOrderDTO> GetOrderByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<GetOrderDTO>> GetOrdersByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GetOrderDTO> CreateOrderAsync(CreateOrderDTO dto, CancellationToken ct = default);
    Task<GetOrderDTO> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDTO dto, CancellationToken ct = default);
    Task DeleteOrderAsync(Guid id, CancellationToken ct = default);
}
