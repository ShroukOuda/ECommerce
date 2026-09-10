namespace ECommerce.Application.DTO.Coupon;

public class ApplyCouponDTO
{
    public Guid CartId { get; set; }
    public string Code { get; set; } = string.Empty;
}