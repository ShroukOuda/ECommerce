using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Notifications;

public class ActiveNotificationPreferencesSpecification : BaseSpecification<NotificationPreference, Guid>
{
    public ActiveNotificationPreferencesSpecification()
        : base(np => np.IsActive)
    {
        AddOrderBy(np => np.Type);
    }
}