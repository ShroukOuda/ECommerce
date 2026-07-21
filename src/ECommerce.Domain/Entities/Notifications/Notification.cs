using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Domain.Entities.Notifications;

public class Notification : BaseEntity<Guid>
{
    public string UserId { get; set; } = null!;
    public NotificationType Type { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime SentAt { get; set; }

    public string? Link { get; set; }

    public User User { get; set; } = null!;
}