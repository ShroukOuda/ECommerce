using System.Globalization;
using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums.Notification;

namespace ECommerce.Application.Services.Notifications;

public class NotificationEventService : INotificationEventService
{
    private readonly INotificationService _notificationService;
    private readonly INotificationPreferenceService _notificationPreferenceService;

    public NotificationEventService(
        INotificationService notificationService,
        INotificationPreferenceService notificationPreferenceService)
    {
        _notificationService = notificationService;
        _notificationPreferenceService = notificationPreferenceService;
    }

    public async Task NotifyBackInStockAsync(string userId, string productName, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.BackInStock,
            "Back in stock",
            $"{productName} is back in stock and ready to order.",
            link);
    }

    public async Task NotifyNewProductAsync(string userId, string productName, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.NewProduct,
            "New product available",
            $"{productName} is now available in the store.",
            link);
    }

    public async Task NotifySecurityAlertAsync(string userId, string? deviceInfo = null, string? link = null)
    {
        var detail = string.IsNullOrWhiteSpace(deviceInfo)
            ? "A security event was detected on your account."
            : $"A security event was detected on your account from {deviceInfo}.";

        await SendIfEnabledAsync(
            userId,
            NotificationType.SecurityAlert,
            "Security alert",
            detail,
            link);
    }

    public async Task NotifyOrderPlacedAsync(string userId, string orderNumber, decimal totalAmount, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.OrderPlaced,
            "Order placed",
            $"Your order #{orderNumber} for {totalAmount.ToString("C", CultureInfo.CurrentCulture)} has been placed successfully.",
            link);
    }

    public async Task NotifyOrderShippedAsync(string userId, string orderNumber, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.OrderShipped,
            "Order shipped",
            $"Your order #{orderNumber} has been shipped and is on the way.",
            link);
    }

    public async Task NotifyOrderDeliveredAsync(string userId, string orderNumber, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.OrderDelivered,
            "Order delivered",
            $"Your order #{orderNumber} has been delivered.",
            link);
    }

    public async Task NotifyPasswordChangedAsync(string userId, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.PasswordChanged,
            "Password changed",
            "Your password was changed successfully. If this wasn’t you, secure your account immediately.",
            link);
    }

    public async Task NotifyLoginFromNewDeviceAsync(string userId, string deviceInfo, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.LoginFromNewDevice,
            "New device login",
            $"We detected a sign-in from a new device: {deviceInfo}.",
            link);
    }

    public async Task NotifyPromotionAsync(string userId, string promotionName, string? link = null)
    {
        await SendIfEnabledAsync(
            userId,
            NotificationType.Promotion,
            "Special offer",
            $"A new promotion is available: {promotionName}.",
            link);
    }

    private async Task SendIfEnabledAsync(
        string userId,
        NotificationType type,
        string title,
        string message,
        string? link)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is required.", nameof(userId));

        var isEnabled = await _notificationPreferenceService.IsEnabledAsync(userId, type);
        if (!isEnabled)
            return;

        await _notificationService.CreateAsync(new CreateNotificationDTO
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Link = link
        });
    }
}
