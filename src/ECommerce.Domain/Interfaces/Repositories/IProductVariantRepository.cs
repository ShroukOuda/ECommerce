using ECommerce.Domain.Entities.Product;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IProductVariantRepository : IGenericRepository<ProductVariant, Guid>
{
    Task<IReadOnlyList<ProductVariant>> GetVariantsByProductIdAsync(Guid productId, CancellationToken ct = default);
}
