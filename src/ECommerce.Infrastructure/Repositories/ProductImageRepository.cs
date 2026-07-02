using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Infrastructure.Repositories;

public class ProductImageRepository : GenericRepository<ProductImage, Guid>, IProductImageRepository
{
    public ProductImageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductImage>> GetImagesByProductIdAsync(
        Guid productId, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(pi => pi.ProductId == productId)
            .OrderByDescending(pi => pi.IsMain)
            .ThenBy(pi => pi.Id)
            .ToListAsync(ct);
    }

    public async Task<ProductImage> GetProductMainImageAsync(
        Guid productId, 
        CancellationToken ct = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.IsMain, ct);
    }

    public async Task<int> CountProductImagesAsync(Guid productId, CancellationToken ct = default)
    {
        return await _dbSet
            .CountAsync(pi => pi.ProductId == productId, ct);
    }
}