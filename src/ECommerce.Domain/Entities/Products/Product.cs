using ECommerce.Domain.Common;
using ECommerce.Domain.Enums.Inventory;
using ECommerce.Domain.Enums.Product;

namespace ECommerce.Domain.Entities.Products;

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
    public virtual Categories.Category Category { get; set; } = null!;
    public virtual Brands.Brand Brand { get; set; } = null!;
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
    public virtual ICollection<Orders.OrderItem> OrderItems { get; set; } = new List<Orders.OrderItem>();
    public virtual ICollection<Carts.CartItem> CartItems { get; set; } = new List<Carts.CartItem>();
    public virtual ICollection<Reviews.ProductReview> ProductReviews { get; set; } = new List<Reviews.ProductReview>();
    public virtual ICollection<Wishlists.Wishlist> Wishlists { get; set; } = new List<Wishlists.Wishlist>();
    public virtual ICollection<Inventories.InventoryHistory> InventoryHistories { get; set; } = new List<Inventories.InventoryHistory>();
    public virtual ICollection<Returns.ReturnItem> ReturnItems { get; set; } = new List<Returns.ReturnItem>();
}