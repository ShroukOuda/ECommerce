using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities.Order;

public class Order : BaseEntity<int>
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    
    //Navigation Properties
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<Review.ProductReview> ProductReviews { get; set; } = new List<Review.ProductReview>();
    public virtual ICollection<Coupon.CouponUsage> CouponUsages { get; set; } = new List<Coupon.CouponUsage>();
}