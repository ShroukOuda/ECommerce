using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Order;

namespace ECommerce.Domain.Entities.Orders;

public class Order : BaseEntity<Guid>
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
    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
    
    //Navigation Properties
    public virtual Users.User? User { get; set; }
    public virtual Users.Address? ShippingAddress { get; set; }
    public virtual Users.Address? BillingAddress { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<Reviews.ProductReview> ProductReviews { get; set; } = new List<Reviews.ProductReview>();
    public virtual ICollection<Coupons.CouponUsage> CouponUsages { get; set; } = new List<Coupons.CouponUsage>();
    public virtual ICollection<Payments.Payment> Payments { get; set; } = new List<Payments.Payment>();
    public virtual ICollection<Shippings.Shipping> Shippings { get; set; } = new List<Shippings.Shipping>();
    public virtual ICollection<Returns.ReturnRequest> ReturnRequests { get; set; } = new List<Returns.ReturnRequest>();
    public virtual ICollection<Returns.ReturnItem> ReturnItems { get; set; } = new List<Returns.ReturnItem>();
}