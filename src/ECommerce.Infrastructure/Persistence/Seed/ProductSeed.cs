namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductSeed
{
    public static List<Product> SeedProducts(ModelBuilder modelBuilder)
    {
        var rnd = new Random();
        var products = new List<Product>();
        for (int i = 1; i <= 100; i++)
        {
            var categoryId = rnd.Next(1, 11); 
            products.Add(new Product
            {
                Id = i,
                Name = $"Product {i} of Category {categoryId}",
                Description = $"This is the description for product {i}.",
                Price = (decimal)Math.Round(rnd.NextDouble() * 500 + 10, 2), 
                StockQuantity = rnd.Next(1, 200),
                SKU = $"SKU-{i:D5}",
                CategoryId = categoryId
            });
        }

        modelBuilder.Entity<Product>().HasData(products.ToArray());

        return products;
    }
}