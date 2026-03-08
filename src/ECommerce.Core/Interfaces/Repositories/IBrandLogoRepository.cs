using ECommerce.Core.Entities.Brand;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IBrandLogoRepository : IGenericRepository<BrandLogo, int>
{
    Task<IReadOnlyList<BrandLogo>> GetLogosByBrandIdAsync(int brandId, CancellationToken ct = default);
}
