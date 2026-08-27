using ECommerce.Application.DTO.Notification;
using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.Interfaces.Notifications;

public interface INotificationPreferenceService
{
    Task<IReadOnlyList<UserNotificationPreferenceDTO>> GetPreferencesAsync(string userId);
    Task UpdatePreferenceAsync(string userId, UpdateNotificationPreferenceDTO dto);
    Task SaveAllPreferencesAsync(string userId, SaveNotificationPreferencesDto dto);
    Task TurnOffAllAsync(string userId);
    Task<bool> IsEnabledAsync(string userId, NotificationType type);
}