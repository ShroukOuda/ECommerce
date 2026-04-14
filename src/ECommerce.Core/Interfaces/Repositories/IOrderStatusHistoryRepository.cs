using ECommerce.Core.Entities.Order;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IOrderStatusHistoryRepository : IGenericRepository<OrderStatusHistory, Guid>
{
    Task<IReadOnlyList<OrderStatusHistory>> GetHistoryByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
