using ECommerce.Domain.Entities.Products;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IProductOptionRepository : IGenericRepository<ProductOption, Guid>
{
    Task<IReadOnlyList<ProductOption>> GetOptionsByProductIdAsync(Guid productId, CancellationToken ct = default);
}
