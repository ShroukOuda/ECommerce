using ECommerce.Domain.Enums.Order;

namespace ECommerce.Domain.Entities.Orders;

public class OrderStatusHistory : BaseEntity<Guid>
{
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    // Who/what made the change: Customer, Admin, Stripe, Shipping or System.
    // The acting user id (when there is one) is stored in CreatedBy.
    public string Source { get; set; } = "System";
    public string? Reason { get; set; }

    //FK
    public Guid OrderId { get; set; }

    //Navigation Properties
    public  Order Order { get; set; } = null!;
}
