namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductSeed
{
    public static void SeedProducts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "iPhone 15",
                Description = "Apple smartphone with A17 chip and dual camera system.",
                Price = 1199.99m,
                StockQuantity = 10,
                SKU = "IPH15-001",
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Men’s Denim Jacket",
                Description = "Stylish blue denim jacket for men.",
                Price = 89.50m,
                StockQuantity = 25,
                SKU = "MDJ-002",
                CategoryId = 2
            },
            new Product
            {
                Id = 3,
                Name = "Microwave Oven",
                Description = "Compact kitchen microwave with digital controls.",
                Price = 230.00m,
                StockQuantity = 15,
                SKU = "MWO-003",
                CategoryId = 3
            },
            new Product
            {
                Id = 4,
                Name = "Facial Cleanser",
                Description = "Gentle foaming face wash suitable for all skin types.",
                Price = 15.75m,
                StockQuantity = 50,
                SKU = "FC-004",
                CategoryId = 4
            },
            new Product
            {
                Id = 5,
                Name = "C# Programming Guide",
                Description = "Comprehensive book on C# and .NET development.",
                Price = 39.99m,
                StockQuantity = 30,
                SKU = "CSHARP-005",
                CategoryId = 5
            }
            );
    }
}