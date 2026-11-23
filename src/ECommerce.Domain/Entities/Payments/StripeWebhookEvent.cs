namespace ECommerce.Domain.Entities.Payments;

public class StripeWebhookEvent : BaseEntity<Guid>
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Provider { get; set; } = "Stripe";
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public Guid? PaymentId { get; set; }
    public string ProcessingResult { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime ProcessedAt { get; set; }

    public Payment? Payment { get; set; }
}