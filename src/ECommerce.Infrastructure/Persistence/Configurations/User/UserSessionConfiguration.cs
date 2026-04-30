using ECommerce.Domain.Entities.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.User;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(us => us.Id);

        builder.Property(us => us.SessionToken)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(us => us.IpAddress)
            .HasMaxLength(45);

        builder.Property(us => us.UserAgent)
            .HasMaxLength(500);

        builder.HasOne(us => us.User)
            .WithMany(u => u.UserSessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}