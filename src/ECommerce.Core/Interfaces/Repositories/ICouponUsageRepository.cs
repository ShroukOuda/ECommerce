using ECommerce.Core.Entities.Coupon;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICouponUsageRepository : IGenericRepository<CouponUsage, Guid>
{
    Task<int> GetUsageCountByUserAsync(Guid couponId, string userId, CancellationToken ct = default);
}
