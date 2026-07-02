using ECommerce.Domain.Entities.Categories;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Category;

public class CategoryImageConfiguration : IEntityTypeConfiguration<CategoryImage>
{
    public void Configure(EntityTypeBuilder<CategoryImage> builder)
    {
        builder.Property(ci => ci.ImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(ci => ci.AltText)
            .HasMaxLength(512);

        builder.HasIndex(ci => ci.CategoryId)
            .HasDatabaseName("IX_CategoryImages_CategoryId");
    }
}