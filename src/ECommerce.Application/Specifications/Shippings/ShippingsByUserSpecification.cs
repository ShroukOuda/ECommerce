using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Shippings;

namespace ECommerce.Application.Specifications.Shippings;

public class ShippingsByUserSpecification : BaseSpecification<Shipping, Guid>
{
    public ShippingsByUserSpecification(string userId)
        : base(s => s.Order.UserId == userId)
    {
        AddOrderByDescending(s => s.CreatedAt);
        AsNoTracking();
    }

    public ShippingsByUserSpecification(Guid shippingId, string userId)
        : base(s => s.Id == shippingId && s.Order.UserId == userId)
    {
        AsNoTracking();
    }

    
}