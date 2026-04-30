using ECommerce.Domain.Entities.Coupon;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICouponRepository : IGenericRepository<Coupon, Guid>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
}
