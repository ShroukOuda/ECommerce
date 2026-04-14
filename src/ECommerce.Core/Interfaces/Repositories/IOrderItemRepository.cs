using ECommerce.Core.Entities.Order;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IOrderItemRepository : IGenericRepository<OrderItem, Guid>
{
    Task<IReadOnlyList<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
