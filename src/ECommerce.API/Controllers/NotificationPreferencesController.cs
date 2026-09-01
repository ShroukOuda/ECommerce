using ECommerce.Application.DTO.Notification;
using ECommerce.Application.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;

[Authorize(Roles = "Admin")]
public class NotificationPreferencesController : BaseController
{
    private readonly INotificationPreferenceAdminService _notificationPreferenceAdminService;

    public NotificationPreferencesController(INotificationPreferenceAdminService notificationPreferenceAdminService)
    {
        _notificationPreferenceAdminService = notificationPreferenceAdminService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var preferences = await _notificationPreferenceAdminService.GetAllAsync();
        return Success(preferences, "Notification preferences retrieved successfully.");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationPreferenceDTO dto)
    {
        var preference = await _notificationPreferenceAdminService.CreateAsync(dto);
        return Created(preference, "Notification preference created successfully.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNotificationPreferenceDTO dto)
    {
        var preference = await _notificationPreferenceAdminService.UpdateAsync(id, dto);
        return Success(preference, "Notification preference updated successfully.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _notificationPreferenceAdminService.DeleteAsync(id);
        return NoContent();
    }
}