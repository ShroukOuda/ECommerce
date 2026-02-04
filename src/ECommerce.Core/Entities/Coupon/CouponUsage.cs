namespace ECommerce.Core.Entities.Coupon;

public class CouponUsage : BaseEntity<int>
{
    public decimal DiscountAmount { get; set; }
    
    //FK
    public int CouponId { get; set; }
    public int OrderId { get; set; }
    
    //Navigation Properties
    public virtual Coupon? Coupon { get; set; }
    public virtual Order.Order? Order { get; set; }
}