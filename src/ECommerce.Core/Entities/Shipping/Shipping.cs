using ECommerce.Core.Enums;
using ECommerce.Core.Enums.Shipping;

namespace ECommerce.Core.Entities.Shipping;

public class Shipping : BaseEntity<int>
{
    public string TrackingNumber { get; set; } = string.Empty;
    public ShippingMethod Method { get; set; }
    public decimal Cost { get; set; }
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public ShippingStatus Status { get; set; } = ShippingStatus.Pending;
    
    //FK
    public int OrderId { get; set; }
    public int AddressId { get; set; }
    
    //Navigation Properties
    public virtual Order.Order? Order { get; set; }
    public virtual User.Address? Address { get; set; }
}