using ECommerce.Domain.Entities.Review;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IProductReviewRepository : IGenericRepository<ProductReview, Guid>
{
    Task<IReadOnlyList<ProductReview>> GetReviewsByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<IReadOnlyList<ProductReview>> GetReviewsByUserIdAsync(string userId, CancellationToken ct = default);
}
