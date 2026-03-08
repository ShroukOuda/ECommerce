using ECommerce.Core.Entities.Shipping;

namespace ECommerce.Infrastructure.Repositories;

public class ShippingRepository : GenericRepository<Shipping, int>, IShippingRepository
{
    public ShippingRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Shipping>> GetShippingsByOrderIdAsync(int orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.OrderId == orderId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
}
