using ECommerce.Core.Entities.Coupon;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICouponUsageRepository : IGenericRepository<CouponUsage, int>
{
    Task<int> GetUsageCountByUserAsync(int couponId, string userId, CancellationToken ct = default);
}
