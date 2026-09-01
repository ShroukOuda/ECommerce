using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Domain.Entities.Notifications;

public class NotificationPreference : BaseEntity<Guid>
{
    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool DefaultEnabled { get; set; } = true;

    public bool IsActive { get; set; } = true;

    public ICollection<UserNotificationPreference> UserPreferences { get; set; }
        = new List<UserNotificationPreference>();
}