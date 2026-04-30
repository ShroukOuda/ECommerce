using ECommerce.Domain.Enums;
using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Shipping;

namespace ECommerce.Domain.Entities.Shipping;

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
    public virtual Order.Order? Order { get; set; }
    public virtual User.Address? Address { get; set; }
}