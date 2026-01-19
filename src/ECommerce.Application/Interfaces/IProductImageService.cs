using ECommerce.Application.DTO.ProductImages;

namespace ECommerce.Application.Interfaces;

public interface IProductImageService
{
    Task<ProductImageDTO> UploadImageAsync(
        UploadProductImageDTO dto, 
        CancellationToken ct = default);
    Task<IReadOnlyList<ProductImageDTO>> GetProductImagesAsync(
        int productId, 
        CancellationToken ct = default);

    Task DeleteProductImageAsync(
        int productId,
        int imageId,
        CancellationToken ct = default);
    Task DeleteAllProductImagesAsync(
        int productId, 
        CancellationToken ct = default);

    Task<ProductImageDTO?> GetImageByIdAsync(
        int imageId,
        CancellationToken ct = default);

    Task<ProductImageDTO?> GetProductMainImageAsync(
        int productId,
        CancellationToken ct = default);
}