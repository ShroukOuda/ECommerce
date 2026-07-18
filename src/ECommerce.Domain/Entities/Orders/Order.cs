
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
    public  User User { get; set; } = null!;
    public  Address ShippingAddress { get; set; } = null!;
    public  Address BillingAddress { get; set; } = null!;
    public  ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public  ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public  ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    public  ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
    public  ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public  ICollection<Shipping> Shippings { get; set; } = new List<Shipping>();
    public  ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
    public  ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}