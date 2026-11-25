using ECommerce.Application.DTO.Order;

namespace ECommerce.Application.Interfaces;

public interface IOrderService
{
    // Customer
    Task<IEnumerable<GetOrderDTO>> GetMyOrdersAsync(
        string userId);

    Task<GetOrderDTO> GetMyOrderByIdAsync(
        Guid orderId,
        string userId);

    Task<GetOrderDTO> CreateOrderAsync(
        string userId,
        CreateOrderDTO dto);

    Task CancelOrderAsync(
        Guid orderId,
        string userId);

    // Admin
    Task<IEnumerable<GetAdminOrderDTO>> GetAllOrdersAsync();

    Task<GetAdminOrderDTO> GetOrderByIdAsync(
        Guid orderId);

    Task<GetAdminOrderDTO> UpdateOrderStatusAsync(
        Guid orderId,
        UpdateOrderStatusDTO dto,
        string? changedBy = null);
}
