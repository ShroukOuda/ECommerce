using ECommerce.Core.Enums;

namespace ECommerce.Core.Entities.Product;

public class Product : BaseEntity<int>
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
    
    public bool IsActive { get; set; }
    
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    
    //FK
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    
    
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
}