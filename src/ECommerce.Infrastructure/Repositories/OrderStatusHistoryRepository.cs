using ECommerce.Core.Entities.Order;

namespace ECommerce.Infrastructure.Repositories;

public class OrderStatusHistoryRepository : GenericRepository<OrderStatusHistory, int>, IOrderStatusHistoryRepository
{
    public OrderStatusHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<OrderStatusHistory>> GetHistoryByOrderIdAsync(int orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(ct);
    }
}
