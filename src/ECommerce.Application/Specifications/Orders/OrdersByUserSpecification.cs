using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Specifications.Orders;

public class OrdersByUserSpecification : BaseSpecification<Order, Guid>
{
    public OrdersByUserSpecification(string userId)
        : base(o => o.UserId == userId)
    {
        AddInclude(o => o.OrderItems);
        AddOrderByDescending(o => o.CreatedAt);
        AsNoTracking();
    }

    
}