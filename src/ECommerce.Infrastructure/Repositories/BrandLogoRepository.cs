using ECommerce.Domain.Entities.Brands;

namespace ECommerce.Infrastructure.Repositories;

public class BrandLogoRepository : GenericRepository<BrandLogo, Guid>, IBrandLogoRepository
{
    public BrandLogoRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<BrandLogo>> GetLogosByBrandIdAsync(Guid brandId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(l => l.BrandId == brandId)
            .OrderBy(l => l.SortOrder)
            .ToListAsync(ct);
    }
}
