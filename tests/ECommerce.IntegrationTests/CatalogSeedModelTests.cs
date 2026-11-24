using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.Persistence.Seed;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Inventories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.IntegrationTests;

public class CatalogSeedModelTests
{
    private static AppDbContext Context() => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task CatalogSeed_IsRepeatableAndAddsMissingRowsWithoutResettingStock()
    {
        using var db = Context();
        var seed = new DataSeeder(db, null!, null!);
        await seed.SeedCatalogAsync();
        (await db.Products.CountAsync()).Should().Be(12);
        (await db.ProductVariants.CountAsync()).Should().Be(16);
        (await db.ProductImages.CountAsync()).Should().Be(12);
        (await db.CategoryImages.CountAsync()).Should().Be(5);
        (await db.BrandLogos.CountAsync()).Should().Be(5);
        var product = await db.Products.FirstAsync(); product.BasePrice = 42m;
        var variant = await db.ProductVariants.FirstAsync(); variant.StockQuantity = 3;
        var image = await db.ProductImages.FirstAsync(); db.ProductImages.Remove(image);
        await db.SaveChangesAsync();
        await seed.SeedCatalogAsync();
        (await db.Products.CountAsync()).Should().Be(12);
        (await db.ProductVariants.CountAsync()).Should().Be(16);
        (await db.ProductImages.CountAsync()).Should().Be(12);
        product.BasePrice.Should().Be(42m); variant.StockQuantity.Should().Be(3);
        var withVariants = await db.ProductVariants.Select(v => v.ProductId).Distinct().CountAsync();
        withVariants.Should().Be(8); // Four real simple products, no hidden Standard rows.
        var imageGroups = (await db.ProductImages.ToListAsync()).GroupBy(x => x.ProductId);
        imageGroups.Should().OnlyContain(g => g.Count(x => x.IsMain) == 1);
        (await db.ProductImages.Select(x => x.ImageUrl).ToListAsync()).Should().OnlyContain(x => x.StartsWith("/Images/seed/"));
    }

    [Fact]
    public void Model_AllowsNullVariantAndEnforcesOneMainImagePerProduct()
    {
        using var db = Context();
        foreach (var (type, property) in new[] {(typeof(CartItem), "VariantId"), (typeof(OrderItem), "ProductVariantId"), (typeof(InventoryHistory), "ProductVariantId")})
            db.Model.FindEntityType(type)!.FindProperty(property)!.IsNullable.Should().BeTrue();
        var image = db.Model.FindEntityType(typeof(ProductImage))!;
        var main = image.GetIndexes().Single(i => i.GetDatabaseName() == "UQ_Product_MainImage");
        main.IsUnique.Should().BeTrue();
        main.Properties.Select(x => x.Name).Should().Equal("ProductId");
        main.GetFilter().Should().Be("IsMain = 1");
    }
}
