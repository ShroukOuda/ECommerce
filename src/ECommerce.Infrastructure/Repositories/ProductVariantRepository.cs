using ECommerce.Domain.Entities.Products;

namespace ECommerce.Infrastructure.Repositories;

public class ProductVariantRepository : GenericRepository<ProductVariant, Guid>, IProductVariantRepository
{
    public ProductVariantRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ProductVariant>> GetVariantsByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(pv => pv.ProductId == productId)
            .Include(pv => pv.ProductImages)
            .Include(pv => pv.ProductVariantOptionValues)
            .ToListAsync(ct);
    }
}
