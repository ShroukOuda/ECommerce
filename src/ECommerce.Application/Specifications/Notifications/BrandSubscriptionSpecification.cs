using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Notifications;


namespace ECommerce.Application.Specifications.Notifications;

public class BrandSubscriptionSpecification : BaseSpecification<BrandSubscription, Guid>
{
    public BrandSubscriptionSpecification(Guid brandId, string userId)
        : base(c => c.BrandId == brandId && c.UserId == userId)
    {
        
    }
    
}