namespace ECommerce.Application.DTO.Payment;

public class GetPaymentStatusHistoryDTO
{
    public Guid Id { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? Reason { get; set; }
    // Id of the user who made the change; empty for Stripe/system changes.
    public string? ChangedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
