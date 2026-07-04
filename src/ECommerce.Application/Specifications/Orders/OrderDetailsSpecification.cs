using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Specifications.Orders;

public class OrderDetailsSpecification : BaseSpecification<Order, Guid>
{
    public OrderDetailsSpecification(Guid orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.OrderItems);
        AddInclude(o => o.ShippingAddress);
        AddInclude(o => o.BillingAddress);
        AddInclude(o => o.Payments);
        AddInclude(o => o.Shippings);
        AddInclude(o => o.OrderStatusHistories);
        AsNoTracking();
    }

    
}