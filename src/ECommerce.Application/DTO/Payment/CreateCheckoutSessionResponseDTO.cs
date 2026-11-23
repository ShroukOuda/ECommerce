namespace ECommerce.Application.DTO.Payment;

public class CreateCheckoutSessionResponseDTO
{
    public Guid PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string CheckoutSessionId { get; set; } = string.Empty;
    public string? CheckoutSessionUrl { get; set; }
    public string? PaymentIntentId { get; set; }
    public decimal Amount { get; set; }
    public long AmountInMinorUnits { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? CheckoutSessionStatus { get; set; }
}