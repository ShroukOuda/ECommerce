using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;
public class NotificationDTO
{
    public Guid Id { get; set; }

    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string? Link { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }
}