namespace ECommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddCategoryAsync(AddCategoryDTO categoryDto, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(UpdateCategoryDTO categoryDto, CancellationToken cancellationToken = default);
    Task DeleteCategoryAsync(int id, CancellationToken cancellationToken = default);
}