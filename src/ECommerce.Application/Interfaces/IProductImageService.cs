using ECommerce.Application.DTO.ProductImages;

namespace ECommerce.Application.Interfaces;

public interface IProductImageService
{
    Task<ProductImageDTO> UploadImageAsync(
        UploadProductImageDTO dto, 
        CancellationToken ct = default);
    Task<IReadOnlyList<ProductImageDTO>> GetProductImagesAsync(
        Guid productId, 
        CancellationToken ct = default);

    Task DeleteProductImageAsync(
        Guid productId,
        Guid imageId,
        CancellationToken ct = default);
    Task DeleteAllProductImagesAsync(
        Guid productId, 
        CancellationToken ct = default);

    Task<ProductImageDTO?> GetImageByIdAsync(
        Guid imageId,
        CancellationToken ct = default);

    Task<ProductImageDTO?> GetProductMainImageAsync(
        Guid productId,
        CancellationToken ct = default);
}