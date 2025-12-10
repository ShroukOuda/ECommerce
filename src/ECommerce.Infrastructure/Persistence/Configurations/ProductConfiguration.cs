using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
        
    }
}