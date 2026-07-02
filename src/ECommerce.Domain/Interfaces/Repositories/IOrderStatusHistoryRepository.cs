using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IOrderStatusHistoryRepository : IGenericRepository<OrderStatusHistory, Guid>
{
    Task<IReadOnlyList<OrderStatusHistory>> GetHistoryByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
