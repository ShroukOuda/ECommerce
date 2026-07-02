using ECommerce.Domain.Entities.Carts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Cart;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Product)
            .WithMany()
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ci => ci.ProductVariant)
            .WithMany(pv => pv.CartItems)
            .HasForeignKey(ci => ci.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(ci => ci.CartItemOptions)
            .WithOne(cio => cio.CartItem)
            .HasForeignKey(cio => cio.CartItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}