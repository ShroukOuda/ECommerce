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

    public int TotalRating { get; set; }

    public int ReviewCount { get; set; }

    public int TotalSales { get; set; }    

    public int ViewCount { get; set; }

    public DateTime? LastViewedAt { get; set; }

    public decimal AverageRating => ReviewCount > 0 ? (decimal)TotalRating / ReviewCount : 0;
    
    public bool IsOnSale => SalePrice < BasePrice && SalePrice > 0;
    public decimal DiscountPercentage => BasePrice > 0 ? ((BasePrice - SalePrice) / BasePrice) * 100 : 0;
    
    
    //FK
    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    
    
    // Navigation Properties
    public  Category Category { get; set; } = null!;
    public  Brand Brand { get; set; } = null!;
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public  ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public  ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();
    public  ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public  ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public  ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
    public  ICollection<InventoryHistory> InventoryHistories { get; set; } = new List<InventoryHistory>();
    public  ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}