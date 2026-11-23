using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Payments;

public class PaymentByPaymentIntentSpecification : BaseSpecification<Payment, Guid>
{
    public PaymentByPaymentIntentSpecification(string paymentIntentId)
        : base(p => p.StripePaymentIntentId == paymentIntentId)
    {
        AsNoTracking();
    }
}