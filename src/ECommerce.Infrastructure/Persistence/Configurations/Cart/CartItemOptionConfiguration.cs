using ECommerce.Core.Entities.Cart;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Cart;

public class CartItemOptionConfiguration : IEntityTypeConfiguration<CartItemOption>
{
    public void Configure(EntityTypeBuilder<CartItemOption> builder)
    {
        builder.HasKey(cio => cio.Id);

        builder.Property(cio => cio.OptionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cio => cio.OptionValue)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(cio => cio.PriceAdjustment)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(cio => cio.CartItem)
            .WithMany(ci => ci.CartItemOptions)
            .HasForeignKey(cio => cio.CartItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cio => cio.ProductOption)
            .WithMany(po => po.CartItemOptions)
            .HasForeignKey(cio => cio.ProductOptionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}