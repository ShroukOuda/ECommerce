using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;

public class UpdateNotificationPreferenceDTO
{
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public bool IsEnabled { get; set; } = true;
}
