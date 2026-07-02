using ECommerce.Domain.Entities.Coupons;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICouponUsageRepository : IGenericRepository<CouponUsage, Guid>
{
    Task<int> GetUsageCountByUserAsync(Guid couponId, string userId, CancellationToken ct = default);
}
