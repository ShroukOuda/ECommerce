using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Notifications;

public class UserNotificationPreferencesSpecification : BaseSpecification<UserNotificationPreference, Guid>
{
    public UserNotificationPreferencesSpecification(string userId)
        : base(unp => unp.UserId == userId)
    {
        AddInclude(unp => unp.NotificationPreference);
        AddOrderBy(unp => unp.NotificationPreferenceId);
    }
}