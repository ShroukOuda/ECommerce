namespace ECommerce.Application.DTO.Coupon;

public class GetCouponDTO : CouponBaseDTO
{
    public Guid Id { get; set; }
    public int UsedCount { get; set; }
    public string Status { get; set; } = string.Empty;
}
