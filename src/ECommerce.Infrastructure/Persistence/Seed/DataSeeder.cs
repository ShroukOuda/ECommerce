using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using ECommerce.Application.Interfaces.Seed;
using ECommerce.Domain.Entities.Brands;
using ECommerce.Domain.Entities.Categories;
using ECommerce.Domain.Entities.Products;
using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.Persistence.Seed.Identity;

namespace ECommerce.Infrastructure.Persistence.Seed;

public class DataSeeder : IDataSeeder
{
    private readonly AppDbContext _context;
    private readonly RoleSeeder _roleSeeder;
    private readonly AdminSeeder _adminSeeder;

    public DataSeeder(
        AppDbContext context,
        RoleSeeder roleSeeder,
        AdminSeeder adminSeeder)
    {
        _context = context;
        _roleSeeder = roleSeeder;
        _adminSeeder = adminSeeder;
    }

    public async Task SeedAsync()
    {
        // Identity
        await _roleSeeder.SeedAsync();
        await _adminSeeder.SeedAsync();

        await SeedCatalogAsync();
    }

    public async Task SeedCatalogAsync()
    {
        // Seed data path
        var seedPath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "Seed",
            "Data");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        await SeedAsync<Category>("Categories.json", _context.Categories, seedPath, options);
        await SeedAsync<Brand>("Brands.json", _context.Brands, seedPath, options);
        await SeedAsync<Product>("Products.json", _context.Products, seedPath, options);
        await SeedAsync<ProductVariant>("ProductVariants.json", _context.ProductVariants, seedPath, options);
        await SeedAsync<ProductImage>("ProductImages.json", _context.ProductImages, seedPath, options);
        await SeedAsync<CategoryImage>("CategoryImages.json", _context.CategoryImages, seedPath, options);
        await SeedAsync<BrandLogo>("BrandLogos.json", _context.BrandLogos, seedPath, options);
        await _context.SaveChangesAsync();
    }

    private static async Task SeedAsync<T>(
        string fileName,
        DbSet<T> dbSet,
        string seedPath,
        JsonSerializerOptions options)
        where T : ECommerce.Domain.Common.BaseEntity<Guid>
    {
        var filePath = Path.Combine(seedPath, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Seed file not found: {filePath}");
        }

        var json = await File.ReadAllTextAsync(filePath);

        var data = JsonSerializer.Deserialize<List<T>>(
            json,
            options);

        if (data is not null && data.Count > 0)
        {
            var existing = (await dbSet.Select(x => x.Id).ToListAsync()).ToHashSet();
            var missing = data.Where(x => !existing.Contains(x.Id)).ToList();
            foreach (var item in missing)
            {
                if (item.CreatedAt == default) item.CreatedAt = DateTime.UtcNow;
                if (item.UpdatedAt == default) item.UpdatedAt = item.CreatedAt;
            }
            // Add only missing IDs; never reset edited prices, stock or customer data.
            await dbSet.AddRangeAsync(missing);
        }
    }
}
