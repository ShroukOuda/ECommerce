using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.DTO.Notification;

public class CreateNotificationDTO
{
    public string UserId { get; set; } = null!;
    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public string? Link { get; set; }
}