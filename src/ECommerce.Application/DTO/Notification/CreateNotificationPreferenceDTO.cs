using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;

public class CreateNotificationPreferenceDTO
{
    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool DefaultEnabled { get; set; } = true;
}