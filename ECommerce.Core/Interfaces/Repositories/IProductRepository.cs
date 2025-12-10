namespace E_Commerece.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    public Task<IEnumerable<Product>> GetAllProductsAsync(ProductParams productParams);
}