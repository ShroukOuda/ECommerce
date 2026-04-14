using ECommerce.Core.Entities.Order;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order, Guid>
{
    Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId, CancellationToken ct = default);
}
