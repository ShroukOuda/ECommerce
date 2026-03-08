using ECommerce.Core.Entities.Product;

namespace ECommerce.Infrastructure.Persistence.Seed;

public static class ProductImageSeed
{
    private static readonly DateTime CreatedAt = new(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedProductImages(ModelBuilder modelBuilder)
    {
        var images = new List<ProductImage>();
        int imageId = 1;

        for (int productId = 1; productId <= 80; productId++)
        {
            // Each product gets 3 images
            for (int img = 1; img <= 3; img++)
            {
                images.Add(new ProductImage
                {
                    Id = imageId,
                    ProductId = productId,
                    ProductVariantId = null,
                    ImageUrl = $"/images/products/product-{productId}-{img}.jpg",
                    AltText = $"Product {productId} Image {img}",
                    IsMain = img == 1,
                    SortOrder = img - 1,
                    UploadedAt = CreatedAt,
                    CreatedAt = CreatedAt,
                    UpdatedAt = CreatedAt,
                    IsDeleted = false
                });
                imageId++;
            }
        }

        modelBuilder.Entity<ProductImage>().HasData(images.ToArray());
    }
}
