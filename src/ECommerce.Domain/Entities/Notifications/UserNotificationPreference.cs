using ECommerce.Domain.Enums.Notification;


namespace ECommerce.Domain.Entities.Notifications;

public class UserNotificationPreference : BaseEntity<Guid>
{
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string UserId { get; set; }  = null!;
    public User User { get; set; } = null!;
}