using ECommerce.Application.DTO.ProductImages;

namespace ECommerce.Application.Interfaces;

public interface IProductImageService
{
    Task<ProductImageDTO> UploadImageAsync(
        Guid productId,
        UploadProductImageDTO dto);
    Task<IReadOnlyList<ProductImageDTO>> GetProductImagesAsync(
        Guid productId);

    Task DeleteProductImageAsync(
        Guid productId,
        Guid imageId);
    Task DeleteAllProductImagesAsync(
        Guid productId);

    Task<ProductImageDTO?> GetImageByIdAsync(
        Guid imageId);

    Task<ProductImageDTO?> GetProductMainImageAsync(
        Guid productId);
}