using ECommerce.Domain.Entities.Product;
using ECommerce.Domain.Specifications;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product, Guid>
{
    public Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
        ProductParams productParams, 
        CancellationToken ct = default);
}