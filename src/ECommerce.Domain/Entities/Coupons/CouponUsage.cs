

namespace ECommerce.Domain.Entities.Coupons;

public class CouponUsage : BaseEntity<Guid>
{
    public decimal DiscountAmount { get; set; }
    
    //FK
    public Guid CouponId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public virtual Coupon Coupon { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}