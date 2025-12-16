namespace ECommerce.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product, int>
{
    public Task<IEnumerable<Product>> GetAllAsync(ProductParams productParams, CancellationToken ct = default);
}