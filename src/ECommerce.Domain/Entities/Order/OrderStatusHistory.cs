using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Order;

namespace ECommerce.Domain.Entities.Order;

public class OrderStatusHistory : BaseEntity<Guid>
{
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    
    //FK
    public Guid OrderId { get; set; }
    
    //Navigation Properties
    public virtual Order? Order { get; set; }
}