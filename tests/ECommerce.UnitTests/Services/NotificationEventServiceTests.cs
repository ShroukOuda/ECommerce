using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Services.Notifications;
using ECommerce.Domain.Enums.Notification;
using FluentAssertions;
using Moq;

namespace ECommerce.UnitTests.Services;

public class NotificationEventServiceTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<INotificationPreferenceService> _preferenceServiceMock;
    private readonly NotificationEventService _service;

    public NotificationEventServiceTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _preferenceServiceMock = new Mock<INotificationPreferenceService>();
        _service = new NotificationEventService(_notificationServiceMock.Object, _preferenceServiceMock.Object);
    }

    [Fact]
    public async Task NotifyNewProductAsync_WhenEnabled_ShouldCreateNotification()
    {
        _preferenceServiceMock.Setup(p => p.IsEnabledAsync("user-1", NotificationType.NewProduct, NotificationChannel.InApp)).ReturnsAsync(true);
        _notificationServiceMock.Setup(n => n.CreateAsync(It.IsAny<CreateNotificationDTO>())).ReturnsAsync(new NotificationDTO());

        await _service.NotifyNewProductAsync("user-1", "Gaming Laptop");

        _notificationServiceMock.Verify(n => n.CreateAsync(It.Is<CreateNotificationDTO>(dto =>
            dto.UserId == "user-1" &&
            dto.Type == NotificationType.NewProduct &&
            dto.Title == "New product available")), Times.Once);
    }

    [Fact]
    public async Task NotifySecurityAlertAsync_WhenDisabled_ShouldNotCreateNotification()
    {
        _preferenceServiceMock.Setup(p => p.IsEnabledAsync("user-1", NotificationType.SecurityAlert, NotificationChannel.InApp)).ReturnsAsync(false);

        await _service.NotifySecurityAlertAsync("user-1", "Chrome on Windows");

        _notificationServiceMock.Verify(n => n.CreateAsync(It.IsAny<CreateNotificationDTO>()), Times.Never);
    }
}
