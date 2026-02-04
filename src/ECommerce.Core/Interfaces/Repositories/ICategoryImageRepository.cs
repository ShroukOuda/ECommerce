using ECommerce.Core.Entities.Category;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICategoryImageRepository : IGenericRepository<CategoryImage, int>
{
    Task<IReadOnlyList<CategoryImage>> GetImagesByCategoryIdAsync(
        int categoryId,
        CancellationToken ct = default);
    
    Task<CategoryImage> GetCategoryImageBySubTypeAsync(
        int categoryId, 
        ImageSubType subType, CancellationToken ct = default);
}