namespace ECommerce.Application.DTO.Coupon;

public class ValidateCouponDTO
{
    public Guid CartId { get; set; }
    public string Code { get; set; } = string.Empty;
}