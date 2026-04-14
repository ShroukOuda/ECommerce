using ECommerce.Core.Entities.Category;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICategoryImageRepository : IGenericRepository<CategoryImage, Guid>
{
    Task<IReadOnlyList<CategoryImage>> GetImagesByCategoryIdAsync(
        Guid categoryId,
        CancellationToken ct = default);
    
    Task<CategoryImage> GetCategoryImageBySubTypeAsync(
        Guid categoryId, 
        ImageSubType subType, CancellationToken ct = default);
}