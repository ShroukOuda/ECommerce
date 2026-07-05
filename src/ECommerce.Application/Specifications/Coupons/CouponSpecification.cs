using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Coupons;

namespace ECommerce.Application.Specifications.Coupons;

public class CouponSpecification : BaseSpecification<Coupon, Guid>
{
    public CouponSpecification(Guid couponId)  
        : base(c => c.Id == couponId)
    {
        AsNoTracking();
    }

    
}