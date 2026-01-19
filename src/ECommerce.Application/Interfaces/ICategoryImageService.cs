using ECommerce.Application.DTO.CategoryImages;

namespace ECommerce.Application.Interfaces;

public interface ICategoryImageService
{
    Task<CategoryImageDTO> UploadImageAsync(
        UploadCategoryImageDTO dto, 
        CancellationToken ct = default);
    
    Task<IReadOnlyList<CategoryImageDTO>> GetCategoryImagesAsync(
        int categoryId, 
        CancellationToken ct = default);
    Task<CategoryImageDTO?> GetCategoryImageBySubTypeAsync(
        int categoryId, 
        ImageSubType subType, 
        CancellationToken ct = default);
    
    Task<CategoryImageDTO?> GetImageByIdAsync(
        int imageId,
        CancellationToken ct = default);
    Task DeleteCategoryImageAsync(
        int categoryId,
        int imageId,
        CancellationToken ct = default);

    Task DeleteAllCategoryImagesAsync(
        int categoryId,
        CancellationToken ct = default);

 
}