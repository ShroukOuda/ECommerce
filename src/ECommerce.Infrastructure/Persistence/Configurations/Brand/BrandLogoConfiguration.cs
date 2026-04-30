using ECommerce.Domain.Entities.Brand;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Brand;

public class BrandLogoConfiguration : IEntityTypeConfiguration<BrandLogo>
{
    public void Configure(EntityTypeBuilder<BrandLogo> builder)
    {
        builder.HasKey(bl => bl.Id);

        builder.Property(bl => bl.ImageUrl)
            .IsRequired();

        builder.HasOne(bl => bl.Brand)
            .WithMany(b => b.BrandLogos)
            .HasForeignKey(bl => bl.BrandId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}