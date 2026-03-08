using ECommerce.Core.Entities.Order;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IOrderStatusHistoryRepository : IGenericRepository<OrderStatusHistory, int>
{
    Task<IReadOnlyList<OrderStatusHistory>> GetHistoryByOrderIdAsync(int orderId, CancellationToken ct = default);
}
