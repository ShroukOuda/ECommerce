using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Reviews;

namespace ECommerce.Application.Specifications.Reviews;

public class ReviewByUserSpecification : BaseSpecification<ProductReview, Guid>
{
    public ReviewByUserSpecification(string userId)
        : base(pr => pr.UserId == userId)
    {
        AddOrderByDescending(pr => pr.CreatedAt);
        AsNoTracking();
    }

    public ReviewByUserSpecification(string userId, Guid id)
        : base(pr => pr.UserId == userId && pr.Id == id)
    {
        AsNoTracking();
    }

    
}