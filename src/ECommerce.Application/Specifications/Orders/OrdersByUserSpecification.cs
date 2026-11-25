using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Specifications.Orders;

public class OrdersByUserSpecification : BaseSpecification<Order, Guid>
{
    public OrdersByUserSpecification(string userId)
        : base(o => o.UserId == userId)
    {
        AddInclude(o => o.OrderItems);
        AddInclude(o => o.OrderStatusHistories);
        AddInclude(o => o.Payments);
        AddInclude(o => o.Shippings);
        AddOrderByDescending(o => o.CreatedAt);
        AsNoTracking();
    }

    
}
