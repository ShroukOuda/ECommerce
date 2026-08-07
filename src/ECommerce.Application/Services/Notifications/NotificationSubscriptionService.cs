using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Specifications.Notifications;
using ECommerce.Domain.Entities.Notifications;
using ECommerce.Domain.Interfaces.Repositories;

namespace ECommerce.Application.Services.Notifications;

public class NotificationSubscriptionService : INotificationSubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationSubscriptionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SubscribeToStockAlertAsync(Guid productId, string userId)
    {
        var subscription = new ProductStockAlert
        {
            ProductId = productId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<ProductStockAlert, Guid>().AddAsync(subscription);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnsubscribeFromStockAlertAsync(Guid productId, string userId)
    {
        var spec = new ProductStockAlertSpecification(productId, userId);
        var subscription = await _unitOfWork.GetRepository<ProductStockAlert, Guid>().GetFirstOrDefaultAsync(spec);

        if (subscription != null)
        {
            _unitOfWork.GetRepository<ProductStockAlert, Guid>().Delete(subscription);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task SubscribeToCategoryAsync(Guid categoryId, string userId)
    {
        var subscription = new CategorySubscription
        {
            CategoryId = categoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.GetRepository<CategorySubscription, Guid>().AddAsync(subscription);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnsubscribeFromCategoryAsync(Guid categoryId, string userId)
    {
        var spec = new CategorySubscriptionSpecification(categoryId, userId);
        var subscription = await _unitOfWork.GetRepository<CategorySubscription, Guid>().GetFirstOrDefaultAsync(spec);

        if (subscription != null)
        {
            _unitOfWork.GetRepository<CategorySubscription, Guid>().Delete(subscription);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}