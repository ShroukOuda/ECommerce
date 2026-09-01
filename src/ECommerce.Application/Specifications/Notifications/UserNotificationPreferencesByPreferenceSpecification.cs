using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Notifications;

public class UserNotificationPreferencesByPreferenceSpecification : BaseSpecification<UserNotificationPreference, Guid>
{
    public UserNotificationPreferencesByPreferenceSpecification(Guid notificationPreferenceId)
        : base(unp => unp.NotificationPreferenceId == notificationPreferenceId)
    {
    }
}