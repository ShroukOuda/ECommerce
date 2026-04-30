using ECommerce.Domain.Entities.Order;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IOrderItemRepository : IGenericRepository<OrderItem, Guid>
{
    Task<IReadOnlyList<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
