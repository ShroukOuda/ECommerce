using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Wishlist;

public class WishlistConfiguration : IEntityTypeConfiguration<Domain.Entities.Wishlist.Wishlist>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Wishlist.Wishlist> builder)
    {
        builder.HasKey(w => w.Id);

        builder.HasIndex(w => new { w.ProductId, w.UserId }).IsUnique();

        builder.Property(w => w.Status)
            .HasConversion<string>();

        builder.HasOne(w => w.Product)
            .WithMany(p => p.Wishlists)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}