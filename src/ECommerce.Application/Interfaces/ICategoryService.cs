using ECommerce.Core.Entities.Category;

namespace ECommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddCategoryAsync(AddCategoryDTO categoryDto, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(UpdateCategoryDTO categoryDto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}