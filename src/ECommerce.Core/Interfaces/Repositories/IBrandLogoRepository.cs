using ECommerce.Core.Entities.Brand;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IBrandLogoRepository : IGenericRepository<BrandLogo, Guid>
{
    Task<IReadOnlyList<BrandLogo>> GetLogosByBrandIdAsync(Guid brandId, CancellationToken ct = default);
}
