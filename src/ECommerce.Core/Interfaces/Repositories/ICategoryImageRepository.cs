namespace ECommerce.Core.Interfaces;

public interface ICategoryImageRepository : IGenericRepository<CategoryImage, int>
{
    Task<IReadOnlyList<CategoryImage>> GetImagesByCategoryIdAsync(
        int categoryId,
        CancellationToken ct = default);
    
    Task<CategoryImage> GetCategoryImageBySubTypeAsync(
        int categoryId, 
        ImageSubType subType, CancellationToken ct = default);
}