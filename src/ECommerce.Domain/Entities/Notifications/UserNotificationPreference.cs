
namespace ECommerce.Domain.Entities.Notifications;

public class UserNotificationPreference : BaseEntity<Guid>
{
    public Guid NotificationPreferenceId { get; set; }

    public bool IsEnabled { get; set; } = true;

    public string UserId { get; set; } = null!;

    public User User { get; set; } = null!;

    public NotificationPreference NotificationPreference { get; set; } = null!;
}