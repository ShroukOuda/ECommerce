using ECommerce.Application.DTO.CategoryImages;

namespace ECommerce.Application.Interfaces;

public interface ICategoryImageService
{
    Task<CategoryImageDTO> UploadImageAsync(
        Guid Id,
        UploadCategoryImageDTO dto);
    
    Task DeleteCategoryImageAsync(
        Guid categoryId,
        Guid imageId);

    Task DeleteAllCategoryImagesAsync(
        Guid categoryId);

 
}