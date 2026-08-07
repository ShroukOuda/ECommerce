using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Notifications;


namespace ECommerce.Application.Specifications.Notifications;

public class NotificationsByUserSpecification : BaseSpecification<Notification, Guid>
{
    public NotificationsByUserSpecification(string userId)
        : base(n => n.UserId == userId)
    {
        
    }

    public NotificationsByUserSpecification(string userId, PaginationParams pagination)
        : base(n => n.UserId == userId)
    {
        ApplyPaging(pagination.PageSize, pagination.PageNumber);
        AddOrderByDescending(n => n.CreatedAt);
        AsNoTracking();
    }


    
}