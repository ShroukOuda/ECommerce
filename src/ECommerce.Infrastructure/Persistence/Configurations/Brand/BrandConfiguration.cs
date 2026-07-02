using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Brand;

public class BrandConfiguration : IEntityTypeConfiguration<Domain.Entities.Brands.Brand>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Brands.Brand> builder)
    {
        builder.HasKey(b => b.Id);

        builder.HasIndex(b => b.Slug).IsUnique();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Status)
            .HasConversion<string>();

        builder.HasMany(b => b.BrandLogos)
            .WithOne(bl => bl.Brand)
            .HasForeignKey(bl => bl.BrandId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Products)
            .WithOne(p => p.Brand)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}