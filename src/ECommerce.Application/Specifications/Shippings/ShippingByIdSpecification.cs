using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Shippings;

namespace ECommerce.Application.Specifications.Shippings;

// Tracked on purpose: used by the update workflow as well as by reads.
public class ShippingByIdSpecification : BaseSpecification<Shipping, Guid>
{
    public ShippingByIdSpecification(Guid shippingId)
        : base(s => s.Id == shippingId)
    {
        AddInclude(s => s.StatusHistories);
    }
}
