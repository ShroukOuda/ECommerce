using ECommerce.Domain.Entities.Inventories;

namespace ECommerce.Infrastructure.Repositories;

public class InventoryHistoryRepository : GenericRepository<InventoryHistory, Guid>, IInventoryHistoryRepository
{
    public InventoryHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InventoryHistory>> GetHistoryByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(ih => ih.ProductId == productId)
            .OrderByDescending(ih => ih.CreatedAt)
            .ToListAsync(ct);
    }
}
