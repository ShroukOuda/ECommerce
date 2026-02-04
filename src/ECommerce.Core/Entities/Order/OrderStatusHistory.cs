using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities.Order;

public class OrderStatusHistory : BaseEntity<int>
{
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
    
    //FK
    public int OrderId { get; set; }
    
    //Navigation Properties
    public virtual Order? Order { get; set; }
}