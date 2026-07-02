using ECommerce.Domain.Entities.Products;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IProductImageRepository : IGenericRepository<ProductImage, Guid>
{
    Task<IReadOnlyList<ProductImage>> GetImagesByProductIdAsync(
        Guid productId, 
        CancellationToken ct = default);
    
    Task<ProductImage> GetProductMainImageAsync(
        Guid productId, 
        CancellationToken ct = default);
    
    Task<int> CountProductImagesAsync(
        Guid productId, 
        CancellationToken ct = default);
}