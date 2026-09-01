using ECommerce.Application.DTO.Notification;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums.Notification;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;

[Authorize]
public class NotificationsController : BaseController
{
    private readonly INotificationService _notificationService;
    private readonly IUserNotificationPreferenceService _notificationPreferenceService;

    public NotificationsController(
        INotificationService notificationService,
        IUserNotificationPreferenceService notificationPreferenceService)
    {
        _notificationService = notificationService;
        _notificationPreferenceService = notificationPreferenceService;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet()]
    public async Task<IActionResult> GetMyNotifications([FromQuery] PaginationParams pagination)
    {
        var notifications = await _notificationService.GetForUserAsync(CurrentUserId, pagination);
        return Success(
            notifications,
            "Notifications retrieved successfully.");
    }

    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread([FromQuery] PaginationParams pagination)
    {
        var notifications = await _notificationService.GetUnreadForUserAsync(CurrentUserId, pagination);
        return Success(
            notifications,
            "Unread notifications retrieved successfully.");
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync(CurrentUserId);
        return Success(count, "Unread count retrieved successfully.");
    }

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        await _notificationService.MarkAsReadAsync(CurrentUserId, notificationId);
        return SuccessMessage("Notification marked as read successfully.");
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        await _notificationService.MarkAllAsReadAsync(CurrentUserId);
        return SuccessMessage("All notifications marked as read successfully.");
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        var preferences = await _notificationPreferenceService.GetPreferencesAsync(CurrentUserId);
        return Success(preferences, "Notification preferences retrieved successfully.");
    }

    [HttpPatch("preferences/{preferenceId:guid}")]
    public async Task<IActionResult> UpdatePreference(Guid preferenceId, [FromBody] UpdateUserNotificationPreferenceDTO dto)
    {
        var updated = await _notificationPreferenceService.UpdatePreferenceAsync(CurrentUserId, preferenceId, dto);
        return Success(updated, "Notification preference updated successfully.");
    }

    [HttpPost("preferences/turn-off-all")]
    public async Task<IActionResult> TurnOffAll()
    {
        await _notificationPreferenceService.TurnOffAllAsync(CurrentUserId);
        return SuccessMessage("Notification preferences turned off successfully.");
    }
}
