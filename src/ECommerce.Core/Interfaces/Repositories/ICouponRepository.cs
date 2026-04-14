using ECommerce.Core.Entities.Coupon;

namespace ECommerce.Core.Interfaces.Repositories;

public interface ICouponRepository : IGenericRepository<Coupon, Guid>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
}
