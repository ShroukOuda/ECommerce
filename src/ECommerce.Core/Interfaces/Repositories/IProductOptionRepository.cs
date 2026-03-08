using ECommerce.Core.Entities.Product;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductOptionRepository : IGenericRepository<ProductOption, int>
{
    Task<IReadOnlyList<ProductOption>> GetOptionsByProductIdAsync(int productId, CancellationToken ct = default);
}
