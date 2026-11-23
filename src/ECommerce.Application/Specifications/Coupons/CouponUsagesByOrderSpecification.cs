using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Coupons;

namespace ECommerce.Application.Specifications.Coupons;

public class CouponUsagesByOrderSpecification : BaseSpecification<CouponUsage, Guid>
{
    public CouponUsagesByOrderSpecification(Guid orderId)
        : base(cu => cu.OrderId == orderId)
    {
        AsNoTracking();
    }
}
