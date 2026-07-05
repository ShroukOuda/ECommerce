using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Orders;

namespace ECommerce.Application.Specifications.Orders;

public class OrderSpecification : BaseSpecification<Order, Guid>
{
    public OrderSpecification(Guid orderId)
        : base(o => o.Id == orderId)
    {
        AsNoTracking();
    }

    
}