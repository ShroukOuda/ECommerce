using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities.Product;

public class ProductVariant : BaseEntity<int>
{
    public string Sku { get; set; } = string.Empty;
    public string VariantName { get; set; } = string.Empty;
    public Dictionary<string, string>? Attributes { get; set; }
    public decimal PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public StockStatus StockStatus { get; set; } = StockStatus.InStock;
    public bool IsActive { get; set; }
    
    public int ProductId { get; set; } //FK
    
    //Navigation Properties
    public virtual Product? Product { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();
    public ICollection<Order.OrderItem> OrderItems { get; set; } = new List<Order.OrderItem>();
    public ICollection<Cart.CartItem> CartItems { get; set; } = new List<Cart.CartItem>();
}