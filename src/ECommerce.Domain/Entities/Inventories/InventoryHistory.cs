
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
    public Guid ProductVariantId { get; set; }
    public string UserId { get; set; } = null!;
    
    //Navigation Properties
    public  Product Product { get; set; } = null!;
    public  ProductVariant ProductVariant { get; set; } = null!;
    public  User User { get; set; } = null!;
}