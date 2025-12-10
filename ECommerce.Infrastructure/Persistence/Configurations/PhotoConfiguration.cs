using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerece.Infrastructure.Persistence.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(photo => photo.Id);
        builder.Property(photo => photo.Id).IsRequired();
        builder.Property(photo => photo.ImageName).HasMaxLength(100);
        builder.HasData(
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