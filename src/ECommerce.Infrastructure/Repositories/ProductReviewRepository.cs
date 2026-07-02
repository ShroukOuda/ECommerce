using ECommerce.Domain.Entities.Reviews;

namespace ECommerce.Infrastructure.Repositories;

public class ProductReviewRepository : GenericRepository<ProductReview, Guid>, IProductReviewRepository
{
    public ProductReviewRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ProductReview>> GetReviewsByProductIdAsync(Guid productId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProductReview>> GetReviewsByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);
    }
}
