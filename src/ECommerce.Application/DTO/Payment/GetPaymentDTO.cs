namespace ECommerce.Application.DTO.Payment;

public class GetPaymentDTO
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public long AmountInMinorUnits { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string? PaymentGateway { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripeCheckoutSessionStatus { get; set; }
    public string? StripeCheckoutSessionUrl { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
