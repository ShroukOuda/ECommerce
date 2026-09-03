using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<GetCategoryDTO>> GetAllCategoriesAsync();
    Task<GetCategoryDetailDTO> GetCategoryByIdAsync(Guid id);
    Task<GetCategoryDTO> AddCategoryAsync(AddCategoryDTO categoryDto);
    Task<GetCategoryDTO> UpdateCategoryAsync(Guid id, UpdateCategoryDTO categoryDto);
    Task DeleteCategoryAsync(Guid id);
}