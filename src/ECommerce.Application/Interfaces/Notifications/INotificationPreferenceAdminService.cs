using ECommerce.Application.DTO.Notification;

namespace ECommerce.Application.Interfaces.Notifications;

public interface INotificationPreferenceAdminService
{
    Task<IReadOnlyList<NotificationPreferenceDTO>> GetAllAsync();

    Task<NotificationPreferenceDTO> GetByIdAsync(Guid id);

    Task<NotificationPreferenceDTO> CreateAsync(CreateNotificationPreferenceDTO dto);

    Task<NotificationPreferenceDTO> UpdateAsync(Guid id, UpdateNotificationPreferenceDTO dto);

    Task DeleteAsync(Guid id);
}