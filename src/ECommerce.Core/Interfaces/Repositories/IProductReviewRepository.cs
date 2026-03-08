using ECommerce.Core.Entities.Review;

namespace ECommerce.Core.Interfaces.Repositories;

public interface IProductReviewRepository : IGenericRepository<ProductReview, int>
{
    Task<IReadOnlyList<ProductReview>> GetReviewsByProductIdAsync(int productId, CancellationToken ct = default);
    Task<IReadOnlyList<ProductReview>> GetReviewsByUserIdAsync(string userId, CancellationToken ct = default);
}
