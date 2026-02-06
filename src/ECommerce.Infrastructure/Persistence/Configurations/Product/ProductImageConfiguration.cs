using ECommerce.Core.Entities.Product;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.Property(pi => pi.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(pi => pi.AltText)
            .HasMaxLength(512);

        builder.HasIndex(pi => pi.ProductId)
            .HasDatabaseName("IX_ProductImages_ProductId");

        builder.HasIndex(pi => pi.IsMain)
            .HasDatabaseName("UQ_Product_MainImage")
            .HasFilter("IsMain = 1")
            .IsUnique();

        builder.HasCheckConstraint(
            "CK_ProductImage_IsMain_Valid",
            "IsMain IN (0, 1)"
        );

        builder.HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade)
            .IsRequired();
    }
}