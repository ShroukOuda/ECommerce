using ECommerce.Domain.Enums;
using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Address;

namespace ECommerce.Domain.Entities.User;

public class Address : BaseEntity<Guid>
{
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public AddressType Type { get; set; }
    public AddressStatus Status { get; set; } = AddressStatus.Active;
    
    //FK
    public string UserId { get; set; }
    
    //Navigation Properties
    public virtual User? User { get; set; }
    public virtual ICollection<Order.Order> ShippingOrders { get; set; } = new List<Order.Order>();
    public virtual ICollection<Order.Order> BillingOrders { get; set; } = new List<Order.Order>();
    public virtual ICollection<Shipping.Shipping> Shippings { get; set; } = new List<Shipping.Shipping>();
}