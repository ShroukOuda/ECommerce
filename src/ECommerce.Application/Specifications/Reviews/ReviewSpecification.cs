using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Reviews;

namespace ECommerce.Application.Specifications.Reviews;

public class ReviewSpecification : BaseSpecification<ProductReview, Guid>
{
    public ReviewSpecification(Guid reviewId)
        : base(pr => pr.Id == reviewId)
    {
        
    }

    
}