using ECommerce.Domain.Entities.Brands;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IBrandLogoRepository : IGenericRepository<BrandLogo, Guid>
{
    Task<IReadOnlyList<BrandLogo>> GetLogosByBrandIdAsync(Guid brandId, CancellationToken ct = default);
}
