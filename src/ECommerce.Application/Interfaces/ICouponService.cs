using ECommerce.Application.DTO.Coupon;

namespace ECommerce.Application.Interfaces;

public interface ICouponService
{
    Task<IEnumerable<GetCouponDTO>> GetAllCouponsAsync();
    Task<GetCouponDTO> GetCouponByIdAsync(Guid id);
    Task<GetCouponDTO?> GetCouponByCodeAsync(string code);
    Task<GetCouponDTO> AddCouponAsync(AddCouponDTO dto);
    Task<GetCouponDTO> UpdateCouponAsync(Guid id, UpdateCouponDTO dto);
    Task DeleteCouponAsync(Guid id);
}
