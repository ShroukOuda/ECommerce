using ECommerce.Domain.Entities.Categories;

namespace ECommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<GetCategoryDTO>> GetAllCategoriesAsync();
    Task<GetCategoryDetailDTO> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GetCategoryDTO> AddCategoryAsync(AddCategoryDTO categoryDto, CancellationToken cancellationToken = default);
    Task<GetCategoryDTO> UpdateCategoryAsync(Guid id, UpdateCategoryDTO categoryDto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}