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
    public  Product Product { get; set; } = null!;
    public  ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public  ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();
    public  ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public  ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public  ICollection<InventoryHistory> InventoryHistories { get; set; } = new List<InventoryHistory>();
}