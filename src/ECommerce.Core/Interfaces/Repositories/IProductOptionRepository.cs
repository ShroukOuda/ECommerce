using ECommerce.Core.Entities.Product;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductOptionRepository : IGenericRepository<ProductOption, Guid>
{
    Task<IReadOnlyList<ProductOption>> GetOptionsByProductIdAsync(Guid productId, CancellationToken ct = default);
}
