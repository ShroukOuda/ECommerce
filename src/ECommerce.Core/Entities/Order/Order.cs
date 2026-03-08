using ECommerce.Core.Enums.Order;

namespace ECommerce.Core.Entities.Order;

public class Order : BaseEntity<int>
{
    public OrderType OrderType { get; set; } = OrderType.Standard;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public string Currency { get; set; } = "USD";
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    
    //FK
    public string UserId { get; set; }
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    
    //Navigation Properties
    public virtual User.User? User { get; set; }
    public virtual User.Address? ShippingAddress { get; set; }
    public virtual User.Address? BillingAddress { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<Review.ProductReview> ProductReviews { get; set; } = new List<Review.ProductReview>();
    public virtual ICollection<Coupon.CouponUsage> CouponUsages { get; set; } = new List<Coupon.CouponUsage>();
    public virtual ICollection<Payment.Payment> Payments { get; set; } = new List<Payment.Payment>();
    public virtual ICollection<Shipping.Shipping> Shippings { get; set; } = new List<Shipping.Shipping>();
    public virtual ICollection<Return.ReturnRequest> ReturnRequests { get; set; } = new List<Return.ReturnRequest>();
    public virtual ICollection<Return.ReturnItem> ReturnItems { get; set; } = new List<Return.ReturnItem>();
}