using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Payments;

public class StripeWebhookEventByEventIdSpecification : BaseSpecification<StripeWebhookEvent, Guid>
{
    public StripeWebhookEventByEventIdSpecification(string eventId)
        : base(e => e.EventId == eventId)
    {
        AsNoTracking();
    }
}