namespace ECommerce.Application.DTO.Notification;

public class SaveNotificationPreferencesDto
{
    public List<UpdateNotificationPreferenceDTO> Preferences { get; set; } = new();
}
