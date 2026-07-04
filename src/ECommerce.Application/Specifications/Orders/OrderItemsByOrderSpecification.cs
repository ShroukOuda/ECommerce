using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Specifications.Orders;

public class OrderItemsByOrderSpecification : BaseSpecification<OrderItem, Guid>
{
    public OrderItemsByOrderSpecification(Guid orderId)
        : base(oi => oi.OrderId == orderId)
    {
        AddInclude(oi => oi.OrderItemOptions);
        AddOrderByDescending(oi => oi.CreatedAt);
        AsNoTracking();
    }

    
}