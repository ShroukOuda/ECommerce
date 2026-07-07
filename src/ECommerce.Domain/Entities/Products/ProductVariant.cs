using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Inventory;
using ECommerce.Domain.Enums.Product;

namespace ECommerce.Domain.Entities.Products;

public class ProductVariant : BaseEntity<Guid>
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
    public Guid ProductId { get; set; } 
    
    //Navigation Properties
    public virtual Product Product { get; set; } = null!;
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<InventoryHistory> InventoryHistories { get; set; } = new List<InventoryHistory>();
}