using ECommerce.Domain.Enums.Payment;

namespace ECommerce.Domain.Entities.Payments;

public class PaymentStatusHistory : BaseEntity<Guid>
{
    public PaymentStatus PaymentStatus { get; set; }

    // Customer, Stripe or System. The acting user id (when there is one) is stored in CreatedBy.
    public string Source { get; set; } = "System";
    public string? Reason { get; set; }

    //FK
    public Guid PaymentId { get; set; }

    //Navigation Properties
    public Payment Payment { get; set; } = null!;
}
