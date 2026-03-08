using ECommerce.Core.Entities.Order;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IOrderItemRepository : IGenericRepository<OrderItem, int>
{
    Task<IReadOnlyList<OrderItem>> GetItemsByOrderIdAsync(int orderId, CancellationToken ct = default);
}
