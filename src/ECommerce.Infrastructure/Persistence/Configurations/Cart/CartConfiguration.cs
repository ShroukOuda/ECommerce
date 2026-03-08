using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Cart;

public class CartConfiguration : IEntityTypeConfiguration<Core.Entities.Cart.Cart>
{
    public void Configure(EntityTypeBuilder<Core.Entities.Cart.Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.GuestToken)
            .HasMaxLength(256);

        builder.Property(c => c.DiscountAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Status)
            .HasConversion<string>();

        builder.HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}