using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IOrderItemRepository : IGenericRepository<OrderItem, Guid>
{
    Task<IReadOnlyList<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
