using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerece.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(200);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasData(
            new Product
            {
                Id = 1,
                Name = "iPhone 15",
                Description = "Apple smartphone with A17 chip and dual camera system.",
                Price = 1199.99f,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Men’s Denim Jacket",
                Description = "Stylish blue denim jacket for men.",
                Price = 89.50f,
                CategoryId = 2
            },
            new Product
            {
                Id = 3,
                Name = "Microwave Oven",
                Description = "Compact kitchen microwave with digital controls.",
                Price = 230.00f,
                CategoryId = 3
            },
            new Product
            {
                Id = 4,
                Name = "Facial Cleanser",
                Description = "Gentle foaming face wash suitable for all skin types.",
                Price = 15.75f,
                CategoryId = 4
            },
            new Product
            {
                Id = 5,
                Name = "C# Programming Guide",
                Description = "Comprehensive book on C# and .NET development.",
                Price = 39.99f,
                CategoryId = 5
            }
        );
    }
}