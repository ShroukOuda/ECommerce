using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Notifications;


namespace ECommerce.Application.Specifications.Notifications;

public class UnreadNotificationsSpecification : BaseSpecification<Notification, Guid>
{
    public UnreadNotificationsSpecification(string userId)
        : base(n => n.UserId == userId && !n.IsRead)
    {
        
    }

    public UnreadNotificationsSpecification(string userId, PaginationParams pagination)
        : base(n => n.UserId == userId && !n.IsRead)
    {
        ApplyPaging(pagination.PageSize, pagination.PageNumber);
        AddOrderByDescending(n => n.CreatedAt);
        AsNoTracking();
    }


    
}