using ECommerce.Domain.Entities.Order;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order, Guid>
{
    Task<Order?> GetOrderWithDetailsAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId, CancellationToken ct = default);
}
