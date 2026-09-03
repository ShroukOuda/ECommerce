using ECommerce.Application.DTO.CategoryImages;

namespace ECommerce.Application.Interfaces;

public interface ICategoryImageService
{
    Task<CategoryImageDTO> UploadImageAsync(
        Guid Id,
        UploadCategoryImageDTO dto);
    
    Task<IReadOnlyList<CategoryImageDTO>> GetCategoryImagesAsync(
        Guid categoryId);
    Task<CategoryImageDTO?> GetCategoryImageBySubTypeAsync(
        Guid categoryId, 
        ImageSubType subType);
    
    Task<CategoryImageDTO?> GetImageByIdAsync(
        Guid categoryId,
        Guid imageId);
    Task DeleteCategoryImageAsync(
        Guid categoryId,
        Guid imageId);

    Task DeleteAllCategoryImagesAsync(
        Guid categoryId);

 
}