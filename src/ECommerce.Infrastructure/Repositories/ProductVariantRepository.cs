using ECommerce.Core.Entities.Product;

namespace ECommerce.Infrastructure.Repositories;

public class ProductVariantRepository : GenericRepository<ProductVariant, int>, IProductVariantRepository
{
    public ProductVariantRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ProductVariant>> GetVariantsByProductIdAsync(int productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(pv => pv.ProductId == productId)
            .Include(pv => pv.ProductImages)
            .Include(pv => pv.ProductVariantOptionValues)
            .ToListAsync(ct);
    }
}
