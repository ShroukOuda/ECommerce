using ECommerce.Domain.Entities.Shipping;

namespace ECommerce.Infrastructure.Repositories;

public class ShippingRepository : GenericRepository<Shipping, Guid>, IShippingRepository
{
    public ShippingRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Shipping>> GetShippingsByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.OrderId == orderId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }
}
