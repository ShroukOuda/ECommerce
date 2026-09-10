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

        // Categories
        if (!await _context.Categories.AnyAsync())
        {
            await SeedAsync<Category>(
                "Categories.json",
                _context.Categories,
                seedPath,
                options);
        }

        // Brands
        if (!await _context.Brands.AnyAsync())
        {
            await SeedAsync<Brand>(
                "Brands.json",
                _context.Brands,
                seedPath,
                options);
        }

        // Products
        if (!await _context.Products.AnyAsync())
        {
            await SeedAsync<Product>(
                "Products.json",
                _context.Products,
                seedPath,
                options);
        }

        await _context.SaveChangesAsync();
    }

    private static async Task SeedAsync<T>(
        string fileName,
        DbSet<T> dbSet,
        string seedPath,
        JsonSerializerOptions options)
        where T : class
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
            await dbSet.AddRangeAsync(data);
        }
    }
}