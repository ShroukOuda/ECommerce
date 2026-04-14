using ECommerce.Core.Enums.Inventory;
using ECommerce.Core.Enums.Product;

namespace ECommerce.Core.Entities.Product;

public class Product : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public int StockQuantity { get; set; }
    public StockStatus StockStatus { get; set; } = StockStatus.InStock;
    public string Sku { get; set; } = string.Empty;
    public bool IsBestSeller { get; set; }
    public bool IsNewArrival { get; set; }
    public bool IsHotDeal { get; set; }
    public bool IsTopRated { get; set; }
    public bool IsFeatured { get; set; }

    public ProductStatus Status { get; set; } = ProductStatus.Published;
    
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    public bool IsOnSale => SalePrice < BasePrice && SalePrice > 0;
    public decimal DiscountPercentage => BasePrice > 0 ? ((BasePrice - SalePrice) / BasePrice) * 100 : 0;
    
    
    //FK
    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    
    
    // Navigation Properties
    public virtual Category.Category? Category { get; set; } 
    public virtual Brand.Brand? Brand { get; set; }
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
    public virtual ICollection<Order.OrderItem> OrderItems { get; set; } = new List<Order.OrderItem>();
    public virtual ICollection<Cart.CartItem> CartItems { get; set; } = new List<Cart.CartItem>();
    public virtual ICollection<Review.ProductReview> ProductReviews { get; set; } = new List<Review.ProductReview>();
    public virtual ICollection<Wishlist.Wishlist> Wishlists { get; set; } = new List<Wishlist.Wishlist>();
    public virtual ICollection<Inventory.InventoryHistory> InventoryHistories { get; set; } = new List<Inventory.InventoryHistory>();
    public virtual ICollection<Return.ReturnItem> ReturnItems { get; set; } = new List<Return.ReturnItem>();
}