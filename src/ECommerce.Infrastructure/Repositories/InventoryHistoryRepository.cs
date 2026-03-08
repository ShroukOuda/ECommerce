using ECommerce.Core.Entities.Inventory;

namespace ECommerce.Infrastructure.Repositories;

public class InventoryHistoryRepository : GenericRepository<InventoryHistory, int>, IInventoryHistoryRepository
{
    public InventoryHistoryRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InventoryHistory>> GetHistoryByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(ih => ih.ProductId == productId)
            .OrderByDescending(ih => ih.CreatedAt)
            .ToListAsync(ct);
    }
}
