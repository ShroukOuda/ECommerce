namespace ECommerce.Infrastructure.Persistence.Seed;

public static class PhotoSeed
{
    public static void SeedPhotos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Photo>().HasData(
            new Photo
            {
                Id = 1,
                ImageName = "Iphone1.jpg",
                ProductId = 1
            },
            new Photo
            {
                Id = 2,
                ImageName = "Iphone2.jpg",
                ProductId = 1
            },
            new Photo
            {
                Id = 3,
                ImageName = "Iphone3.jpg",
                ProductId = 1
            },
            new Photo
            {
                Id = 4,
                ImageName = "Iphone4.jpg",
                ProductId = 1
            }
            );
    }
}