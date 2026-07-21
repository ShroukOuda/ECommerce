using System.Reflection;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Entities.Inventories;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Entities.Returns;
using ECommerce.Domain.Entities.Reviews;
using ECommerce.Domain.Entities.Shippings;
using ECommerce.Domain.Entities.Users;
using ECommerce.Domain.Entities.Wishlists;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Context;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    //Category
    public  DbSet<Category> Categories { get; set; }
    public  DbSet<CategoryImage> CategoryImages { get; set; }
    
    //Product
    public  DbSet<Product> Products { get; set; }
    public  DbSet<ProductImage> ProductImages { get; set; }
    public  DbSet<ProductOption> ProductOptions { get; set; }
    public  DbSet<ProductOptionValue> ProductOptionValues { get; set; }
    public  DbSet<ProductVariant> ProductVariants { get; set; }
    public  DbSet<ProductVariantOptionValue> ProductVariantOptionValues { get; set; }
    
    //Brand
    public  DbSet<Brand> Brands { get; set; }
    public  DbSet<BrandLogo> BrandLogos { get; set; }
    
    //Order
    public  DbSet<Order> Orders { get; set; }
    public  DbSet<OrderItem> OrderItems { get; set; }
    public  DbSet<OrderItemOption> OrderItemOptions { get; set; }
    public  DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    
    //Cart
    public  DbSet<Cart> Carts { get; set; }
    public  DbSet<CartItem> CartItems { get; set; }
    public  DbSet<CartItemOption> CartItemOptions { get; set; }
    
    //Coupon
    public  DbSet<Coupon> Coupons { get; set; }
    public  DbSet<CouponUsage> CouponUsages { get; set; }
    
    //Review
    public  DbSet<ProductReview> ProductReviews { get; set; }
    public  DbSet<ReviewHelpfulVote> ReviewHelpfulVotes { get; set; }
    
    //Wishlist
    public  DbSet<Wishlist> Wishlists { get; set; }
    
    //Inventory
    public  DbSet<InventoryHistory> InventoryHistories { get; set; }
    
    //Payment
    public  DbSet<Payment> Payments { get; set; }
    
    //Return
    public  DbSet<ReturnRequest> ReturnRequests { get; set; }
    public  DbSet<ReturnItem> ReturnItems { get; set; }
    
    //Shipping
    public  DbSet<Shipping> Shippings { get; set; }
    
    //User
    public  DbSet<UserSession> UserSessions { get; set; }
    public  DbSet<Address> Addresses { get; set; }

    //Notification
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotificationPreference> UserNotificationPreferences { get; set; }
    public DbSet<CategorySubscription> CategorySubscriptions { get; set; }
    public DbSet<ProductStockAlert> ProductStockAlerts { get; set; }    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
       
    }
}