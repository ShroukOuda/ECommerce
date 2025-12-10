using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerece.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(200);
        builder.HasData(
            new Category
            {
                Id = 1,
                Name = "Electronics",
                Description = "Devices, gadgets, and electronic accessories"
            },
            new Category
            {
                Id = 2,
                Name = "Clothing",
                Description = "Men’s, women’s, and children’s fashion items"
            },
            new Category
            {
                Id = 3,
                Name = "Home & Kitchen",
                Description = "Appliances, furniture, and home improvement tools"
            },
            new Category
            {
                Id = 4,
                Name = "Beauty & Health",
                Description = "Cosmetics, skincare, and healthcare products"
            },
            new Category
            {
                Id = 5,
                Name = "Books",
                Description = "Educational, fiction, and non-fiction books"
            }
            );
    }
}