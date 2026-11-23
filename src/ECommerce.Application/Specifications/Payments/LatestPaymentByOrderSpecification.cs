using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Payments;

public class LatestPaymentByOrderSpecification : BaseSpecification<Payment, Guid>
{
    public LatestPaymentByOrderSpecification(Guid orderId) : base(p => p.OrderId == orderId)
    {
        AddOrderByDescending(p => p.CreatedAt);
        ApplyPaging(1, 1);
        // Used by mutation workflows; keep this graph tracked.
    }
}
