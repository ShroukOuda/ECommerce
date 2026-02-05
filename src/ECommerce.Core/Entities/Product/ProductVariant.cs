using ECommerce.Core.Enums.Inventory;
using ECommerce.Core.Enums.Product;

namespace ECommerce.Core.Entities.Product;

public class ProductVariant : BaseEntity<int>
{
    public string Sku { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public Dictionary<string, string>? Attributes { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string? Material { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public StockStatus StockStatus { get; set; } = StockStatus.InStock;
    public ProductVariantStatus Status { get; set; } = ProductVariantStatus.Active;
    
    //FK
    public int ProductId { get; set; } 
    
    //Navigation Properties
    public virtual Product? Product { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();
    public ICollection<Order.OrderItem> OrderItems { get; set; } = new List<Order.OrderItem>();
    public ICollection<Cart.CartItem> CartItems { get; set; } = new List<Cart.CartItem>();
    public ICollection<Inventory.InventoryHistory> InventoryHistories { get; set; } = new List<Inventory.InventoryHistory>();
}