using ECommerce.Core.Entities.Order;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order, int>
{
    Task<Order?> GetOrderWithDetailsAsync(int orderId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId, CancellationToken ct = default);
}
