using ECommerce.Application.DTO.Notification;
using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.Interfaces.Notifications;

public interface IUserNotificationPreferenceService
{
    Task<IReadOnlyList<UserNotificationPreferenceDTO>> GetPreferencesAsync(string userId);

    Task<UserNotificationPreferenceDTO> UpdatePreferenceAsync(
        string userId,
        Guid preferenceId,
        UpdateUserNotificationPreferenceDTO dto);

    Task TurnOffAllAsync(string userId);

    Task<bool> IsEnabledAsync(string userId, Guid preferenceId);

    Task<bool> IsEnabledAsync(string userId, NotificationType type, NotificationChannel channel);
}

public interface INotificationPreferenceService : IUserNotificationPreferenceService
{
}