using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductConfiguration : IEntityTypeConfiguration<Domain.Entities.Products.Product>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Products.Product> builder)
    {
        builder.HasKey(p => p.Id);

      

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Slug).IsRequired().HasMaxLength(250);
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.Sku).IsRequired().HasMaxLength(100);

        builder.Property(p => p.BasePrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.SalePrice).HasColumnType("decimal(18,2)");


        builder.Property(p => p.StockStatus).HasConversion<string>().HasMaxLength(50);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(50);

        builder.Property(p => p.TotalSales).HasDefaultValue(0);
        builder.Property(p => p.ReviewCount).HasDefaultValue(0);
        builder.Property(p => p.TotalRating).HasDefaultValue(0);
        builder.Property(p => p.ViewCount).HasDefaultValue(0);

        builder.Property(p => p.LastViewedAt).HasDefaultValue(null);

        // Ignore computed C# properties
        builder.Ignore(p => p.IsOnSale);
        builder.Ignore(p => p.DiscountPercentage);
        builder.Ignore(p => p.AverageRating);


        // FK Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation collections
        builder.HasMany(p => p.ProductImages)
            .WithOne(pi => pi.Product)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductVariants)
            .WithOne(pv => pv.Product)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.ProductOptions)
            .WithOne(po => po.Product)
            .HasForeignKey(po => po.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.ProductReviews)
            .WithOne(pr => pr.Product)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.Wishlists)
            .WithOne(w => w.Product)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.InventoryHistories)
            .WithOne(ih => ih.Product)
            .HasForeignKey(ih => ih.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.ReturnItems)
            .WithOne(ri => ri.Product)
            .HasForeignKey(ri => ri.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasMany(p => p.ProductStockAlerts)
            .WithOne(PSA => PSA.Product)
            .HasForeignKey(PSA => PSA.ProductId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.Property(p => p.IsDeleted).HasDefaultValue(false);

        //Indexes
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.Sku).IsUnique();

        
        builder.HasIndex(p => p.CategoryId).HasDatabaseName("IX_Products_CategoryId");
        builder.HasIndex(p => p.BrandId).HasDatabaseName("IX_Products_BrandId");
       
    }
}