using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Coupons;

namespace ECommerce.Application.Specifications.Coupons;

public class CouponUsageByUserSpecification : BaseSpecification<CouponUsage, Guid>
{
    public CouponUsageByUserSpecification(Guid couponId, string userId)
        : base(cu => cu.CouponId == couponId && cu.UserId == userId)
    {
        AddOrderByDescending(pr => pr.CreatedAt);
        AsNoTracking();
    }

    
}