using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Enums.Notification;
namespace ECommerce.Application.Specifications.Notifications;

public class NotificationPreferenceSpecification : BaseSpecification<NotificationPreference, Guid>
{
    public NotificationPreferenceSpecification()
    {
        AddOrderBy(np => np.Type);
    }

    public NotificationPreferenceSpecification(Guid id)
        : base(np => np.Id == id)
    {
    }

    public NotificationPreferenceSpecification(NotificationType type, NotificationChannel channel)
        : base(np => np.Type == type && np.Channel == channel)
    {
    }
    
}