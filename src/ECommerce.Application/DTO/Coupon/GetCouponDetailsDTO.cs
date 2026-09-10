using ECommerce.Domain.Enums.Coupon;

namespace ECommerce.Application.DTO.Coupon;


public class GetCouponDetailsDTO : CouponBaseDTO
{
    public Guid Id { get; set; }
    public int UsedCount { get; set; }
    public CouponStatus Status { get; set; } 
    public int RemainingUsage =>
        UsageLimit > 0
            ? Math.Max(UsageLimit - UsedCount, 0)
            : 0;
    public bool IsExpired =>
        DateTime.UtcNow > ValidUntil;

    public bool IsCurrentlyValid =>
        Status == CouponStatus.Active &&
        DateTime.UtcNow >= ValidFrom &&
        DateTime.UtcNow <= ValidUntil &&
        (UsageLimit <= 0 || UsedCount < UsageLimit);
}