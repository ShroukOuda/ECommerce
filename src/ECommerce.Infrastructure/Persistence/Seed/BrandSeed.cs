using ECommerce.Core.Entities.Brand;
using ECommerce.Core.Enums.Brand;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class BrandSeed
{
    // Brand IDs: 1–30
    public static void SeedBrands(ModelBuilder modelBuilder)
    {
        var createdAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var brands = new List<Brand>
        {
            // Electronics brands
            new() { Id = 1,  Name = "Apple",          Slug = "apple",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 2,  Name = "Samsung",        Slug = "samsung",        Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 3,  Name = "Sony",           Slug = "sony",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 4,  Name = "LG",             Slug = "lg",             Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 5,  Name = "Huawei",         Slug = "huawei",         Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 6,  Name = "Xiaomi",         Slug = "xiaomi",         Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 7,  Name = "OnePlus",        Slug = "oneplus",        Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 8,  Name = "Dell",           Slug = "dell",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 9,  Name = "HP",             Slug = "hp",             Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 10, Name = "Lenovo",         Slug = "lenovo",         Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 11, Name = "Asus",           Slug = "asus",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 12, Name = "Acer",           Slug = "acer",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 13, Name = "Bose",           Slug = "bose",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 14, Name = "JBL",            Slug = "jbl",            Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 15, Name = "Canon",          Slug = "canon",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 16, Name = "Nikon",          Slug = "nikon",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 17, Name = "GoPro",          Slug = "gopro",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 18, Name = "DJI",            Slug = "dji",            Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 19, Name = "Anker",          Slug = "anker",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 20, Name = "Logitech",       Slug = "logitech",       Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            // Fashion brands
            new() { Id = 21, Name = "Nike",           Slug = "nike",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 22, Name = "Adidas",         Slug = "adidas",         Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 23, Name = "Zara",           Slug = "zara",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 24, Name = "H&M",            Slug = "h-and-m",        Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 25, Name = "Levis",          Slug = "levis",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            // Home brands
            new() { Id = 26, Name = "IKEA",           Slug = "ikea",           Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 27, Name = "Philips",        Slug = "philips",        Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 28, Name = "Dyson",          Slug = "dyson",          Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            // Other
            new() { Id = 29, Name = "Amazon Basics",  Slug = "amazon-basics",  Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
            new() { Id = 30, Name = "Generic",        Slug = "generic",        Status = BrandStatus.Active,   CreatedAt = createdAt, UpdatedAt = createdAt, IsDeleted = false },
        };

        modelBuilder.Entity<Brand>().HasData(brands.ToArray());

        // Brand logos
        var logos = new List<BrandLogo>();
        for (int i = 1; i <= 30; i++)
        {
            logos.Add(new BrandLogo
            {
                Id = i,
                BrandId = i,
                ImageUrl = $"/images/brands/{brands[i - 1].Slug}-logo.png",
                AltText = $"{brands[i - 1].Name} Logo",
                IsMain = true,
                SortOrder = 0,
                UploadedAt = createdAt,
                CreatedAt = createdAt,
                UpdatedAt = createdAt,
                IsDeleted = false
            });
        }

        modelBuilder.Entity<BrandLogo>().HasData(logos.ToArray());
    }
}
