using ECommerce.Core.Entities.Brand;

namespace ECommerce.Infrastructure.Repositories;

public class BrandLogoRepository : GenericRepository<BrandLogo, int>, IBrandLogoRepository
{
    public BrandLogoRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BrandLogo>> GetLogosByBrandIdAsync(int brandId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(l => l.BrandId == brandId)
            .OrderBy(l => l.SortOrder)
            .ToListAsync(ct);
    }
}
