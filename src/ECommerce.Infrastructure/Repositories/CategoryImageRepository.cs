using ECommerce.Domain.Entities.Category;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryImageRepository : GenericRepository<CategoryImage, Guid>, ICategoryImageRepository
{
    public CategoryImageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<CategoryImage>> GetImagesByCategoryIdAsync(
        Guid categoryId, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(ci => ci.CategoryId == categoryId)
            .OrderBy(ci => ci.SubType)
            .ToListAsync(ct);
    }

    public async Task<CategoryImage> GetCategoryImageBySubTypeAsync(
        Guid categoryId, 
        ImageSubType subType, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ci => ci.CategoryId == categoryId && ci.SubType == subType, ct);
    }
}