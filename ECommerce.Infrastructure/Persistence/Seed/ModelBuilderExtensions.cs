namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        CategorySeed.SeedCategories(modelBuilder);
        ProductSeed.SeedProducts(modelBuilder);
        PhotoSeed.SeedPhotos(modelBuilder);
    }
}