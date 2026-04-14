namespace ECommerce.Application.DTO.Payment;

public class GetPaymentDTO
{
    public Guid Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
}
