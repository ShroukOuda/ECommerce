namespace ECommerce.Application.Interfaces.Notifications;

public interface INotificationSubscriptionService
{
    Task SubscribeToStockAlertAsync(Guid productId, string userId);

    Task UnsubscribeFromStockAlertAsync(Guid productId, string userId);

    Task SubscribeToCategoryAsync(Guid categoryId, string userId);

    Task UnsubscribeFromCategoryAsync(Guid categoryId, string userId);
}