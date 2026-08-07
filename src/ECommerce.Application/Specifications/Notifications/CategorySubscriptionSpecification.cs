using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Notifications;


namespace ECommerce.Application.Specifications.Notifications;

public class CategorySubscriptionSpecification : BaseSpecification<CategorySubscription, Guid>
{
    public CategorySubscriptionSpecification(Guid categoryId, string userId)
        : base(c => c.CategoryId == categoryId && c.UserId == userId)
    {
        
    }
    
}