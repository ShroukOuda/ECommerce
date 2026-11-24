using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Payments;

public class PaymentByCheckoutSessionSpecification : BaseSpecification<Payment, Guid>
{
    public PaymentByCheckoutSessionSpecification(string checkoutSessionId)
        : base(p => p.StripeCheckoutSessionId == checkoutSessionId)
    {
        // Used by mutation workflows; keep this graph tracked.
    }
}
