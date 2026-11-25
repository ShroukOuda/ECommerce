using ECommerce.Domain.Enums.Shipping;

namespace ECommerce.Domain.Entities.Shippings;

public class ShippingStatusHistory : BaseEntity<Guid>
{
    public ShippingStatus ShippingStatus { get; set; }

    // Admin or System. The acting user id (when there is one) is stored in CreatedBy.
    public string Source { get; set; } = "System";
    public string? Reason { get; set; }

    //FK
    public Guid ShippingId { get; set; }

    //Navigation Properties
    public Shipping Shipping { get; set; } = null!;
}
