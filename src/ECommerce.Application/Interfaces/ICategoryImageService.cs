using ECommerce.Application.DTO.CategoryImages;

namespace ECommerce.Application.Interfaces;

public interface ICategoryImageService
{
    Task<CategoryImageDTO> UploadImageAsync(
        UploadCategoryImageDTO dto, 
        CancellationToken ct = default);
    
    Task<IReadOnlyList<CategoryImageDTO>> GetCategoryImagesAsync(
        Guid categoryId, 
        CancellationToken ct = default);
    Task<CategoryImageDTO?> GetCategoryImageBySubTypeAsync(
        Guid categoryId, 
        ImageSubType subType, 
        CancellationToken ct = default);
    
    Task<CategoryImageDTO?> GetImageByIdAsync(
        Guid imageId,
        CancellationToken ct = default);
    Task DeleteCategoryImageAsync(
        Guid categoryId,
        Guid imageId,
        CancellationToken ct = default);

    Task DeleteAllCategoryImagesAsync(
        Guid categoryId,
        CancellationToken ct = default);

 
}