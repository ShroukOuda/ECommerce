using ECommerce.Core.Entities.Category;

namespace ECommerce.Infrastructure.Persistence.Seed;
public static class CategorySeed
{
    public static List<Category> SeedCategories(ModelBuilder modelBuilder)
    {
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", Description = "Devices and gadgets" },
            new Category { Id = 2, Name = "Books", Description = "All genres of books" },
            new Category { Id = 3, Name = "Clothing", Description = "Men and women apparel" },
            new Category { Id = 4, Name = "Home & Kitchen", Description = "Household items" },
            new Category { Id = 5, Name = "Sports", Description = "Sports and outdoors" },
            new Category { Id = 6, Name = "Toys", Description = "Toys for kids of all ages" },
            new Category { Id = 7, Name = "Beauty", Description = "Cosmetics and skincare" },
            new Category { Id = 8, Name = "Automotive", Description = "Car accessories" },
            new Category { Id = 9, Name = "Pet Supplies", Description = "Products for pets" },
            new Category { Id = 10, Name = "Music & Instruments", Description = "Instruments and accessories" }
        };

        modelBuilder.Entity<Category>().HasData(categories.ToArray());

        return categories;
    }
}