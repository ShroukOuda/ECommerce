using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Coupons;

namespace ECommerce.Application.Specifications.Coupons;

public class CouponByCodeSpecification : BaseSpecification<Coupon, Guid>
{
    public CouponByCodeSpecification(string code)  
        : base(c => c.Code == code)
    {
        AsNoTracking();
    }

    
}