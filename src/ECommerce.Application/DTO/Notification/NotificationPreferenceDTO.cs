using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;

public class NotificationPreferenceDTO
{
    public Guid Id { get; set; }

    public NotificationType Type { get; set; }

    public NotificationChannel Channel { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool DefaultEnabled { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}