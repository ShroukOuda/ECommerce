using ECommerce.Core.Enums.Inventory;

namespace ECommerce.Core.Entities.Inventory;

public class InventoryHistory : BaseEntity<int>
{
    public int QuantityChange { get; set; }
    public int NewQuantity { get; set; }
    public InventoryChangeType ChangeType { get; set; }
    public string? ReferencedId { get; set; }
    public string? ReferencedType { get; set; }
    public string? Notes { get; set; }

    //FK
    public int ProductId { get; set; }
    public int? ProductVariantId { get; set; }
    public string? UserId { get; set; }
    
    //Navigation Properties
    public virtual Product.Product? Product { get; set; }
    public virtual Product.ProductVariant? ProductVariant { get; set; }
    public virtual User.User? User { get; set; }
}