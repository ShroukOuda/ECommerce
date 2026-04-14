using ECommerce.Core.Entities.Coupon;

namespace ECommerce.Infrastructure.Repositories;

public class CouponUsageRepository : GenericRepository<CouponUsage, Guid>, ICouponUsageRepository
{
    public CouponUsageRepository(AppDbContext context) : base(context) { }

    public async Task<int> GetUsageCountByUserAsync(Guid couponId, string userId, CancellationToken ct = default)
    {
        return await _dbSet.CountAsync(cu => cu.CouponId == couponId && cu.UserId == userId, ct);
    }
}
