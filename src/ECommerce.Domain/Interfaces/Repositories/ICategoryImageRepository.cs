using ECommerce.Domain.Entities.Category;
using ECommerce.Domain.Enums.Media;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICategoryImageRepository : IGenericRepository<CategoryImage, Guid>
{
    Task<IReadOnlyList<CategoryImage>> GetImagesByCategoryIdAsync(
        Guid categoryId,
        CancellationToken ct = default);
    
    Task<CategoryImage> GetCategoryImageBySubTypeAsync(
        Guid categoryId, 
        ImageSubType subType, CancellationToken ct = default);
}