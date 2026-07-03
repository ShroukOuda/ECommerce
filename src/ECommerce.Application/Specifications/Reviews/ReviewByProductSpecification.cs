using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Reviews;

namespace ECommerce.Application.Specifications.Reviews;

public class ReviewByProductSpecification : BaseSpecification<ProductReview, Guid>
{
    public ReviewByProductSpecification(Guid productId)
        : base(pr => pr.ProductId == productId)
    {
        AddOrderByDescending(pr => pr.CreatedAt);
        AsNoTracking();
    }

    
}