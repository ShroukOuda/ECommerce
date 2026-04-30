using ECommerce.Domain.Entities.Coupon;

namespace ECommerce.Infrastructure.Repositories;

public class CouponRepository : GenericRepository<Coupon, Guid>, ICouponRepository
{
    public CouponRepository(AppDbContext context) : base(context) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, ct);
    }
}
