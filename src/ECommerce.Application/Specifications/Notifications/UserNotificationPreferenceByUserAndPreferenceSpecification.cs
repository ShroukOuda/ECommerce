using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Specifications.Base;

namespace ECommerce.Application.Specifications.Notifications;

public class UserNotificationPreferenceByUserAndPreferenceSpecification
    : BaseSpecification<UserNotificationPreference, Guid>
{
    public UserNotificationPreferenceByUserAndPreferenceSpecification(string userId, Guid notificationPreferenceId)
        : base(unp => unp.UserId == userId && unp.NotificationPreferenceId == notificationPreferenceId)
    {
        AddInclude(unp => unp.NotificationPreference);
    }
}