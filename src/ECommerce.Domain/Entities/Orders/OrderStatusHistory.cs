
using ECommerce.Domain.Enums.Order;

namespace ECommerce.Domain.Entities.Orders;

public class OrderStatusHistory : BaseEntity<Guid>
{
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    
    //FK
    public Guid OrderId { get; set; }
    
    //Navigation Properties
    public  Order Order { get; set; } = null!;
}