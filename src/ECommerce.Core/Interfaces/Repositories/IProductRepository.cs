using ECommerce.Core.Entities.Product;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product, int>
{
    public Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
        ProductParams productParams, 
        CancellationToken ct = default);
}