using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;

public class UserNotificationPreferenceDTO
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool IsEnabled { get; set; }
    public string UserId { get; set; } = string.Empty;
}
