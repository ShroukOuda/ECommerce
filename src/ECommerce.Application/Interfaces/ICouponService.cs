using ECommerce.Application.DTO.Coupon;

namespace ECommerce.Application.Interfaces;

public interface ICouponService
{
    Task<IEnumerable<GetCouponDTO>> GetAllCouponsAsync(CancellationToken ct = default);
    Task<GetCouponDTO> GetCouponByIdAsync(Guid id, CancellationToken ct = default);
    Task<GetCouponDTO?> GetCouponByCodeAsync(string code, CancellationToken ct = default);
    Task AddCouponAsync(AddCouponDTO dto, CancellationToken ct = default);
    Task UpdateCouponAsync(UpdateCouponDTO dto, CancellationToken ct = default);
    Task DeleteCouponAsync(Guid id, CancellationToken ct = default);
}
