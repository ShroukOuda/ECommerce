namespace ECommerce.Application.DTO.Payment;

public class StripeWebhookEventDTO
{
    public string EventId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? CheckoutSessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? PaymentStatus { get; set; }
    public string? CheckoutSessionStatus { get; set; }
    public string? ClientReferenceId { get; set; }
    public long? AmountTotal { get; set; }
    public string? Currency { get; set; }
    public string? PayloadSummary { get; set; }
}