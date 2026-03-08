using ECommerce.Core.Entities.Order;

namespace ECommerce.Infrastructure.Repositories;

public class OrderItemRepository : GenericRepository<OrderItem, int>, IOrderItemRepository
{
    public OrderItemRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<OrderItem>> GetItemsByOrderIdAsync(int orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.OrderItemOptions)
            .ToListAsync(ct);
    }
}
