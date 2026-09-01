using ECommerce.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations.Notifications;

public class UserNotificationPreferenceConfiguration
    : IEntityTypeConfiguration<UserNotificationPreference>
{
    public void Configure(EntityTypeBuilder<UserNotificationPreference> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsEnabled)
            .HasDefaultValue(true);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.NotificationPreferenceId
        }).IsUnique();

        builder.HasIndex(x => x.NotificationPreferenceId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserNotificationPreferences)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.NotificationPreference)
            .WithMany(x => x.UserPreferences)
            .HasForeignKey(x => x.NotificationPreferenceId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}