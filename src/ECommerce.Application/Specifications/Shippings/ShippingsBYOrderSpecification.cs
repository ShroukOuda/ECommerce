using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Shippings;

namespace ECommerce.Application.Specifications.Shippings;

public class ShippingsByOrderSpecification : BaseSpecification<Shipping, Guid>
{
    public ShippingsByOrderSpecification(Guid orderId)
        : base(s => s.OrderId == orderId)
    {
        AddOrderByDescending(s => s.CreatedAt);
        AsNoTracking();
    }

    
}