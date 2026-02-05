using ECommerce.Core.Enums;
using ECommerce.Core.Enums.Address;

namespace ECommerce.Core.Entities.User;

public class Address : BaseEntity<int>
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
    public int UserId { get; set; }
    
    //Navigation Properties
    public virtual User? User { get; set; }
    public virtual ICollection<Order.Order> Orders { get; set; } = new List<Order.Order>();
    public virtual ICollection<Shipping.Shipping> Shippings { get; set; } = new List<Shipping.Shipping>();
}