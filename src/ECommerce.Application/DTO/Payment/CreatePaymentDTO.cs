namespace ECommerce.Application.DTO.Payment;

public class CreatePaymentDTO
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
}
