using ECommerce.Core.Enums.Order;

namespace ECommerce.Core.Entities.Order;

public class OrderStatusHistory : BaseEntity<Guid>
{
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    
    //FK
    public Guid OrderId { get; set; }
    
    //Navigation Properties
    public virtual Order? Order { get; set; }
}