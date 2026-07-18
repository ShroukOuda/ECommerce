

namespace ECommerce.Domain.Entities.Coupons;

public class CouponUsage : BaseEntity<Guid>
{
    public decimal DiscountAmount { get; set; }
    
    //FK
    public Guid CouponId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public  Coupon Coupon { get; set; } = null!;
    public  Order Order { get; set; } = null!;
    public  User User { get; set; } = null!;
}