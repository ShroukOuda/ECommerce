using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Specifications.Notifications;
using ECommerce.Domain.Entities.Notifications;


namespace ECommerce.Application.Services.Notifications;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

     public async Task<NotificationDTO> CreateAsync(CreateNotificationDTO dto)
    {

        var notification = _mapper.Map<Notification>(dto);
        notification.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.GetRepository<Notification, Guid>().AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<NotificationDTO>(notification);
    }

    public async Task<PaginatedResult<NotificationDTO>> GetForUserAsync(
        string userId, PaginationParams pagination)
    {
        var spec  = new NotificationsByUserSpecification(userId, pagination);
        var notifications = await _unitOfWork.GetRepository<Notification, Guid>().GetAllAsync(spec);
        var mappedNotifications = _mapper.Map<List<NotificationDTO>>(notifications);
        var count = await _unitOfWork.GetRepository<Notification, Guid>().CountAsync(new NotificationsByUserSpecification(userId));

        return new PaginatedResult<NotificationDTO>(mappedNotifications, count, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<PaginatedResult<NotificationDTO>> GetUnreadForUserAsync(
        string userId, PaginationParams pagination)
    {
        var spec  = new UnreadNotificationsSpecification(userId, pagination);
        var notifications = await _unitOfWork.GetRepository<Notification, Guid>().GetAllAsync(spec);
        var mappedNotifications = _mapper.Map<List<NotificationDTO>>(notifications);
        var count = await _unitOfWork.GetRepository<Notification, Guid>().CountAsync(new UnreadNotificationsSpecification(userId));

        return new PaginatedResult<NotificationDTO>(mappedNotifications, count, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        int count = await _unitOfWork.GetRepository<Notification, Guid>().CountAsync(
                        new UnreadNotificationsSpecification(userId));
        return count;
    }

    public async Task MarkAsReadAsync(string userId, Guid notificationId)
    {
        var n = await _unitOfWork.GetRepository<Notification, Guid>().GetByIdAsync(notificationId)
            ?? throw new Exception("Notification not found");

        if (n.UserId != userId) throw new UnauthorizedAccessException();

        n.IsRead = true;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var spec  = new UnreadNotificationsSpecification(userId);
        var notifications = await _unitOfWork.GetRepository<Notification, Guid>().GetAllAsync(spec);
        foreach (var n in notifications) n.IsRead = true;
        await _unitOfWork.SaveChangesAsync();
    }


}