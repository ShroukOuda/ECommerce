namespace ECommerce.Infrastructure.Persistence.Seed;
public static class CategorySeed
{
    public static void SeedCategories(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
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