using ECommerce.Application.DTO.Notification;

namespace ECommerce.Application.Interfaces.Notifications;

public interface INotificationService
{
    Task<NotificationDTO> CreateAsync(CreateNotificationDTO notificationDto);
    Task<PaginatedResult<NotificationDTO>> GetForUserAsync(string userId, PaginationParams pagination);
    Task<PaginatedResult<NotificationDTO>> GetUnreadForUserAsync(string userId, PaginationParams pagination);
    Task<int> GetUnreadCountAsync(string userId);
    Task MarkAsReadAsync(string userId, Guid notificationId);
    Task MarkAllAsReadAsync(string userId);
}