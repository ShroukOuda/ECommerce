using ECommerce.Domain.Entities.Products;

namespace ECommerce.Infrastructure.Repositories;

public class ProductOptionRepository : GenericRepository<ProductOption, Guid>, IProductOptionRepository
{
    public ProductOptionRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ProductOption>> GetOptionsByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(po => po.ProductId == productId)
            .Include(po => po.ProductOptionValues)
            .OrderBy(po => po.SortOrder)
            .ToListAsync(ct);
    }
}
