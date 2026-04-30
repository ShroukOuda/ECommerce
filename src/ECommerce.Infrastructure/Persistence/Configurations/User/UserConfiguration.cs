using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.User;

public class UserConfiguration : IEntityTypeConfiguration<Domain.Entities.User.User>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.User.User> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.CountryCode)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(u => u.Status)
            .HasConversion<string>();

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(2048);

        builder.HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserSessions)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.Wishlists)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.Carts)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.CouponUsages)
            .WithOne(cu => cu.User)
            .HasForeignKey(cu => cu.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.ProductReviews)
            .WithOne(pr => pr.User)
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.ReturnRequests)
            .WithOne(rr => rr.User)
            .HasForeignKey(rr => rr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.InventoryHistories)
            .WithOne(ih => ih.User)
            .HasForeignKey(ih => ih.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(u => u.Payments)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}