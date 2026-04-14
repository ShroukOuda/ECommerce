namespace ECommerce.Core.Entities.Coupon;

public class CouponUsage : BaseEntity<Guid>
{
    public decimal DiscountAmount { get; set; }
    
    //FK
    public Guid CouponId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; }
    
    //Navigation Properties
    public virtual Coupon? Coupon { get; set; }
    public virtual Order.Order? Order { get; set; }
    public virtual User.User? User { get; set; }
}