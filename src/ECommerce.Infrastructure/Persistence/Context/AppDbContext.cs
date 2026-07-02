using System.Reflection;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Entities.Inventories;
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
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<CategoryImage> CategoryImages { get; set; }
    
    //Product
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<ProductOption> ProductOptions { get; set; }
    public virtual DbSet<ProductOptionValue> ProductOptionValues { get; set; }
    public virtual DbSet<ProductVariant> ProductVariants { get; set; }
    public virtual DbSet<ProductVariantOptionValue> ProductVariantOptionValues { get; set; }
    
    //Brand
    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<BrandLogo> BrandLogos { get; set; }
    
    //Order
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<OrderItemOption> OrderItemOptions { get; set; }
    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    
    //Cart
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<CartItemOption> CartItemOptions { get; set; }
    
    //Coupon
    public virtual DbSet<Coupon> Coupons { get; set; }
    public virtual DbSet<CouponUsage> CouponUsages { get; set; }
    
    //Review
    public virtual DbSet<ProductReview> ProductReviews { get; set; }
    public virtual DbSet<ReviewHelpfulVote> ReviewHelpfulVotes { get; set; }
    
    //Wishlist
    public virtual DbSet<Wishlist> Wishlists { get; set; }
    
    //Inventory
    public virtual DbSet<InventoryHistory> InventoryHistories { get; set; }
    
    //Payment
    public virtual DbSet<Payment> Payments { get; set; }
    
    //Return
    public virtual DbSet<ReturnRequest> ReturnRequests { get; set; }
    public virtual DbSet<ReturnItem> ReturnItems { get; set; }
    
    //Shipping
    public virtual DbSet<Shipping> Shippings { get; set; }
    
    //User
    public virtual DbSet<UserSession> UserSessions { get; set; }
    public virtual DbSet<Address> Addresses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
       
    }
}