using ECommerce.Core.Entities.Product;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductImageRepository : IGenericRepository<ProductImage, int>
{
    Task<IReadOnlyList<ProductImage>> GetImagesByProductIdAsync(
        int productId, 
        CancellationToken ct = default);
    
    Task<ProductImage> GetProductMainImageAsync(
        int productId, 
        CancellationToken ct = default);
    
    Task<int> CountProductImagesAsync(
        int productId, 
        CancellationToken ct = default);
}