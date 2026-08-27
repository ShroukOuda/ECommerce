using ECommerce.Application.DTO.Notification;
using ECommerce.Application.DTO.Pagination;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Domain.Enums.Notification;

namespace ECommerce.API.Controllers;

public class NotificationsController : BaseController
{
    private readonly INotificationService _notificationService;
    private readonly INotificationPreferenceService _notificationPreferenceService;

    public NotificationsController(
        INotificationService notificationService,
        INotificationPreferenceService notificationPreferenceService)
    {
        _notificationService = notificationService;
        _notificationPreferenceService = notificationPreferenceService;
    }

    [HttpGet("get-for-user/{userId}")]
    public async Task<IActionResult> GetForUser(string userId, [FromQuery] PaginationParams pagination)
    {
        var notifications = await _notificationService.GetForUserAsync(userId, pagination);
        return Ok(notifications);
    }

    [HttpGet("get-unread/{userId}")]
    public async Task<IActionResult> GetUnread(string userId, [FromQuery] PaginationParams pagination)
    {
        var notifications = await _notificationService.GetUnreadForUserAsync(userId, pagination);
        return Ok(notifications);
    }

    [HttpGet("unread-count/{userId}")]
    public async Task<IActionResult> GetUnreadCount(string userId)
    {
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    [HttpPost("mark-as-read/{userId}/{notificationId}")]
    public async Task<IActionResult> MarkAsRead(string userId, Guid notificationId)
    {
        await _notificationService.MarkAsReadAsync(userId, notificationId);
        return Ok(new ResponseAPI(200, "Notification marked as read successfully"));
    }

    [HttpPost("mark-all-as-read/{userId}")]
    public async Task<IActionResult> MarkAllAsRead(string userId)
    {
        await _notificationService.MarkAllAsReadAsync(userId);
        return Ok(new ResponseAPI(200, "All notifications marked as read successfully"));
    }

    [HttpGet("preferences/{userId}")]
    public async Task<IActionResult> GetPreferences(string userId)
    {
        var preferences = await _notificationPreferenceService.GetPreferencesAsync(userId);
        return Ok(preferences);
    }

    [HttpPost("preferences/{userId}/update")]
    public async Task<IActionResult> UpdatePreference(string userId, [FromBody] UpdateNotificationPreferenceDTO dto)
    {
        await _notificationPreferenceService.UpdatePreferenceAsync(userId, dto);
        return Ok(new ResponseAPI(200, "Notification preference updated successfully"));
    }

    [HttpPost("preferences/{userId}/save")]
    public async Task<IActionResult> SaveAll(string userId, [FromBody] SaveNotificationPreferencesDto dto)
    {
        await _notificationPreferenceService.SaveAllPreferencesAsync(userId, dto);
        return Ok(new ResponseAPI(200, "Notification preferences saved successfully"));
    }

    [HttpPost("preferences/{userId}/turn-off-all")]
    public async Task<IActionResult> TurnOffAll(string userId)
    {
        await _notificationPreferenceService.TurnOffAllAsync(userId);
        return Ok(new ResponseAPI(200, "All notification preferences disabled successfully"));
    }

    [HttpGet("preferences/{userId}/is-enabled/{type}")]
    public async Task<IActionResult> IsEnabled(string userId, NotificationType type)
    {
        var enabled = await _notificationPreferenceService.IsEnabledAsync(userId, type);
        return Ok(enabled);
    }
}
