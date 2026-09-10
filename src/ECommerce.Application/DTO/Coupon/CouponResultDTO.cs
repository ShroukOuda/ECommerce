namespace ECommerce.Application.DTO.Coupon;

public class CouponResultDTO
{
    public string Code { get; set; } = string.Empty;

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAfterDiscount { get; set; }
}