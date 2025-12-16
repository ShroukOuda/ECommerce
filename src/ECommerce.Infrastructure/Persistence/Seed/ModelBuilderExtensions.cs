namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        var categories = CategorySeed.SeedCategories(modelBuilder);
        var products = ProductSeed.SeedProducts(modelBuilder);
        PhotoSeed.SeedPhotos(modelBuilder, products, categories);
    }
}