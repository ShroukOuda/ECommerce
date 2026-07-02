using ECommerce.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.User;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(us => us.Id);

        builder.Property(us => us.RefreshToken)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(us => us.IpAddress)
            .IsRequired()
            .HasMaxLength(45);

        builder.Property(us => us.UserAgent)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(us => us.DeviceInfo)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(us => us.CreatedAt)
            .IsRequired();
        
        builder.Property(us => us.RefreshTokenExpiresAt)
            .IsRequired();
        
        builder.Property(us => us.RevokedAt)
            .IsRequired(false);

        builder.Property(us => us.LastUsedAt)
            .IsRequired(false);

        builder.Property(us => us.ReplacedByToken)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(us => us.IsActive)
            .IsRequired();

        builder.HasIndex(us => us.UserId);

        builder.HasOne(us => us.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}