using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities.Coupon;

public class Coupon : BaseEntity<int>
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; } = DiscountType.Percentage;
    public decimal DiscountValue { get; set; }
    public decimal MinPurchaseAmount { get; set; }
    public decimal MaxDiscountAmount { get; set; }
    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public int PerUserLimit { get; set; } = 1;
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsActive { get; set; }
    
    //Navigation Properties
    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();

}