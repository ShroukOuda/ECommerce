using ECommerce.Core.Entities.Product;

namespace ECommerce.Infrastructure.Repositories;

public class ProductOptionRepository : GenericRepository<ProductOption, int>, IProductOptionRepository
{
    public ProductOptionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ProductOption>> GetOptionsByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(po => po.ProductId == productId)
            .Include(po => po.ProductOptionValues)
            .OrderBy(po => po.SortOrder)
            .ToListAsync(ct);
    }
}
