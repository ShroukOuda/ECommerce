
using ECommerce.Domain.Enums.Address;

namespace ECommerce.Domain.Entities.Users;

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
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<Order> ShippingOrders { get; set; } = new List<Orders.Order>();
    public virtual ICollection<Order> BillingOrders { get; set; } = new List<Orders.Order>();
    public virtual ICollection<Shipping> Shippings { get; set; } = new List<Shippings.Shipping>();
}