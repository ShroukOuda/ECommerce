using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Product;

public class ProductConfiguration : IEntityTypeConfiguration<Core.Entities.Product.Product>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Product.Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired();
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Description).HasMaxLength(200);
        builder.Property(p => p.BasePrice).IsRequired().HasColumnType("decimal(18,2)");
        
    }
}