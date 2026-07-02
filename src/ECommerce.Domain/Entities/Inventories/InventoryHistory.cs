using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Inventory;

namespace ECommerce.Domain.Entities.Inventories;

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
    public virtual Products.Product? Product { get; set; }
    public virtual Products.ProductVariant? ProductVariant { get; set; }
    public virtual Users.User? User { get; set; }
}