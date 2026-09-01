using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;

public class UserNotificationPreferenceDTO
{
    public Guid Id { get; set; }

    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
