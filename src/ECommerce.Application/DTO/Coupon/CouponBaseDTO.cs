namespace ECommerce.Application.DTO.Coupon;

public class CouponBaseDTO
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DiscountType { get; set; } = "Percentage";
    public decimal DiscountValue { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public int UsageLimit { get; set; }
    public int PerUserLimit { get; set; } = 1;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
}
