namespace ECommerce.Application.Interfaces.Notifications;

public interface INotificationEventService
{
    Task NotifyBackInStockAsync(string userId, string productName, string? link = null);
    Task NotifyNewProductAsync(string userId, string productName, string? link = null);
    Task NotifySecurityAlertAsync(string userId, string? deviceInfo = null, string? link = null);
    Task NotifyOrderPlacedAsync(string userId, string orderNumber, decimal totalAmount, string? link = null);
    Task NotifyOrderShippedAsync(string userId, string orderNumber, string? link = null);
    Task NotifyOrderDeliveredAsync(string userId, string orderNumber, string? link = null);
    Task NotifyPasswordChangedAsync(string userId, string? link = null);
    Task NotifyLoginFromNewDeviceAsync(string userId, string deviceInfo, string? link = null);
    Task NotifyPromotionAsync(string userId, string promotionName, string? link = null);
}
