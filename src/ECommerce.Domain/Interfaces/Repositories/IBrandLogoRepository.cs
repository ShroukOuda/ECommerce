using ECommerce.Domain.Entities.Brand;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IBrandLogoRepository : IGenericRepository<BrandLogo, Guid>
{
    Task<IReadOnlyList<BrandLogo>> GetLogosByBrandIdAsync(Guid brandId, CancellationToken ct = default);
}
