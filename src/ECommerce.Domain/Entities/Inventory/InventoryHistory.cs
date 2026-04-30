using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Inventory;

namespace ECommerce.Domain.Entities.Inventory;

public class InventoryHistory : BaseEntity<Guid>
{
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public InventoryChangeType ChangeType { get; set; }
    public string? ReferencedId { get; set; }
    public string? ReferencedType { get; set; }
    public string? Notes { get; set; }

    //FK
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string? UserId { get; set; }
    
    //Navigation Properties
    public virtual Product.Product? Product { get; set; }
    public virtual Product.ProductVariant? ProductVariant { get; set; }
    public virtual User.User? User { get; set; }
}