using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Specifications.Orders;

public class OrderStatusHistoryByOrderSpecification : BaseSpecification<OrderStatusHistory, Guid>
{
    public OrderStatusHistoryByOrderSpecification(Guid orderId)
        : base(h => h.OrderId == orderId)
    {
        AddOrderByDescending(h => h.CreatedAt);
        AsNoTracking();
    }

    
}