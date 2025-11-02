using E_Commerece.Core.DTO;
using E_Commerece.Core.Entites.Product;

namespace E_Commerece.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(AddCategoryDTO categoryDto);
    Task UpdateCategoryAsync(UpdateCategoryDTO categoryDto);
    Task DeleteCategoryAsync(int id);
}