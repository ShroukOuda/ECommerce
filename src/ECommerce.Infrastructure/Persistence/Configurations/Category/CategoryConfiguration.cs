using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Category;

public class CategoryConfiguration : IEntityTypeConfiguration<Domain.Entities.Category.Category>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Category.Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Slug).IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Self-referencing relationship
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation collections
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.CategoryImages)
            .WithOne(ci => ci.Category)
            .HasForeignKey(ci => ci.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.IsDeleted).HasDefaultValue(false);
    }
}