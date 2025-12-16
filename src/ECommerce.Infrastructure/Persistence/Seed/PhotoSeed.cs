namespace ECommerce.Infrastructure.Persistence.Seed;

public static class PhotoSeed
{
    public static void SeedPhotos(ModelBuilder modelBuilder, List<Product> products, List<Category> categories)
    {
        var photos = new List<Photo>();
        int photoId = 1;
        
        foreach (var product in products)
        {
            for (int j = 1; j <= 4; j++) 
            {
                photos.Add(new Photo
                {
                    Id = photoId++,
                    Url = $"https://picsum.photos/seed/product{product.Id}_{j}/400/400",
                    AltText = $"Photo {j} of {product.Name}",
                    IsMain = j == 1,
                    Type = PhotoType.ProductImage,
                    ProductId = product.Id,
                    CategoryId = product.CategoryId, 
                    SubType = null
                });
            }
        }

    
        foreach (var category in categories)
        {
            for (int k = 1; k <= 3; k++) 
            {
                photos.Add(new Photo
                {
                    Id = photoId++,
                    Url = $"https://picsum.photos/seed/category{category.Id}_{k}/400/400",
                    AltText = $"Photo {k} of {category.Name}",
                    IsMain = k == 1, 
                    Type = PhotoType.CategoryMedia,
                    CategoryId = category.Id,
                    ProductId = null,
                    SubType = k switch
                    {
                        1 => PhotoSubType.CategoryIcon,
                        2 => PhotoSubType.CategoryBanner,
                        3 => PhotoSubType.CategoryThumbnail,
                        _ => PhotoSubType.CategoryThumbnail 
                    }
                });
            }
        }


        modelBuilder.Entity<Photo>().HasData(photos.ToArray());
    }
}