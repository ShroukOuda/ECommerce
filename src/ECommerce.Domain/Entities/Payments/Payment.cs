
using ECommerce.Domain.Enums.Payment;

namespace ECommerce.Domain.Entities.Payments;

public class Payment : BaseEntity<Guid>
{
    public string TransactionId { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string? PaymentGateway { get; set; } 
    public string? GatewayTransactionId { get; set; }
    public string? GatewayResponse { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripeCheckoutSessionStatus { get; set; }
    public string? StripeCheckoutSessionUrl { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public long AmountInMinorUnits { get; set; }
    public DateTime? PaidAt { get; set; }
    
    //FK
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public  Order Order { get; set; } = null!;
    public  User User { get; set; } = null!;
}