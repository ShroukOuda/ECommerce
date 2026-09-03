using ECommerce.Application.DTO.Order;

namespace ECommerce.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<GetOrderDTO>> GetAllOrdersAsync();
    Task<GetOrderDTO> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<GetOrderDTO>> GetOrdersByUserIdAsync(string userId);
    Task<GetOrderDTO> CreateOrderAsync(CreateOrderDTO dto);
    Task<GetOrderDTO> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDTO dto);
    Task DeleteOrderAsync(Guid id);
}
