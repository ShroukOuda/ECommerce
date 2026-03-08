using ECommerce.Core.Entities.Product;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductVariantRepository : IGenericRepository<ProductVariant, int>
{
    Task<IReadOnlyList<ProductVariant>> GetVariantsByProductIdAsync(int productId, CancellationToken ct = default);
}
