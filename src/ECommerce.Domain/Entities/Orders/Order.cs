
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
    public string UserId { get; set; } = null!;
    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
    
    //Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Address ShippingAddress { get; set; } = null!;
    public virtual Address BillingAddress { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
    public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}