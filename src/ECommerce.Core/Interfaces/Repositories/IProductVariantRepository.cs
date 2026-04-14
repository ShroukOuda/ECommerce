using ECommerce.Core.Entities.Product;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductVariantRepository : IGenericRepository<ProductVariant, Guid>
{
    Task<IReadOnlyList<ProductVariant>> GetVariantsByProductIdAsync(Guid productId, CancellationToken ct = default);
}
