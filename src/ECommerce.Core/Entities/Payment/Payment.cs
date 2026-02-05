using ECommerce.Core.Enums.Payment;

namespace ECommerce.Core.Entities.Payment;

public class Payment : BaseEntity<int>
{
    public string TransactionId { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string? PaymentGateway { get; set; } 
    public string? GatewayTransactionId { get; set; }
    public string? GatewayResponse { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    
    //FK
    public int OrderId { get; set; }
    public int UserId { get; set; }
    
    //Navigation Properties
    public virtual Order.Order? Order { get; set; }
    public virtual User.User? User { get; set; }
}