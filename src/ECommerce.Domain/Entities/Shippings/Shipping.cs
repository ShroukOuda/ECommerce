using ECommerce.Domain.Enums.Shipping;

namespace ECommerce.Domain.Entities.Shippings;

public class Shipping : BaseEntity<Guid>
{
    public string TrackingNumber { get; set; } = string.Empty;
    public ShippingMethod Method { get; set; }
    public decimal Cost { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public ShippingStatus Status { get; set; } = ShippingStatus.Pending;
    
    //FK
    public Guid OrderId { get; set; }
    public Guid AddressId { get; set; }
    
    //Navigation Properties
    public  Order Order { get; set; } = null!;
    public  Address Address { get; set; } = null!;
}