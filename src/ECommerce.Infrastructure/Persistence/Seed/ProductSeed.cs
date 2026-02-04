using ECommerce.Core.Entities.Product;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductSeed
{
    public static List<Product> SeedProducts(ModelBuilder modelBuilder)
    {
        var rnd = new Random();
        if (rnd == null) throw new ArgumentNullException(nameof(rnd));
        var products = new List<Product>();
        for (int i = 1; i <= 100; i++)
        {
            var categoryId = rnd.Next(1, 11);
            var item = new Product
            {
                Id = i,
                Name = $"Product {i} of Category {categoryId}",
                Description = $"This is the description for product {i}.",
                BasePrice = (decimal)Math.Round(rnd.NextDouble() * 500 + 10, 2), 
                StockQuantity = rnd.Next(1, 200),
                CategoryId = categoryId
            };
            item.Sku = $"SKU-{i:D5}";
            products.Add(item);
        }

        modelBuilder.Entity<Product>().HasData(products.ToArray());

        return products;
    }
}