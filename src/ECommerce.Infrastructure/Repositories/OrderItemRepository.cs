using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Infrastructure.Repositories;

public class OrderItemRepository : GenericRepository<OrderItem, Guid>, IOrderItemRepository
{
    public OrderItemRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.OrderItemOptions)
            .ToListAsync(ct);
    }
}
