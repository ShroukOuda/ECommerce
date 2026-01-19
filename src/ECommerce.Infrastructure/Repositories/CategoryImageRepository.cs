namespace ECommerce.Infrastructure.Repositories;

public class CategoryImageRepository : GenericRepository<CategoryImage, int>, ICategoryImageRepository
{
    public CategoryImageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<CategoryImage>> GetImagesByCategoryIdAsync(
        int categoryId, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(ci => ci.CategoryId == categoryId)
            .OrderBy(ci => ci.SubType)
            .ToListAsync(ct);
    }

    public async Task<CategoryImage> GetCategoryImageBySubTypeAsync(
        int categoryId, 
        ImageSubType subType, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ci => ci.CategoryId == categoryId && ci.SubType == subType, ct);
    }
}