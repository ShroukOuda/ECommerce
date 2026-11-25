using ECommerce.Application.DTO.Coupon;

namespace ECommerce.Application.Interfaces;

public interface ICouponService
{
    Task<IEnumerable<GetCouponDTO>> GetAllCouponsAsync();
    Task<GetCouponDetailsDTO> GetCouponByIdAsync(Guid id);
    Task<GetCouponDTO?> GetCouponByCodeAsync(string code);
    Task<GetCouponDTO> AddCouponAsync(AddCouponDTO dto);
    Task<GetCouponDTO> UpdateCouponAsync(Guid id, UpdateCouponDTO dto);
    Task DeleteCouponAsync(Guid id);
    Task<CouponResultDTO> ValidateCouponAsync(string userId, ValidateCouponDTO dto);
    Task<CouponResultDTO> ValidateCouponForSubtotalAsync(string userId, Guid cartId, string code, decimal subtotal);
    Task<CouponResultDTO> ApplyCouponAsync(string userId, ApplyCouponDTO dto);
}
