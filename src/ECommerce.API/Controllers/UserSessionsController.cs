using ECommerce.Application.DTO.UserSession;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerce.API.Controllers;


[Authorize]
public class UserSessionsController : BaseController
{
    private readonly IUserSessionService _sessionService;

    public UserSessionsController(IUserSessionService sessionService)
        => _sessionService = sessionService;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSessions()
    {
        var sessions = await _sessionService.GetActiveSessionsAsync(CurrentUserId);
        return Success(
            sessions,
            "Active sessions retrieved successfully.");
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllSessions()
    {
        var sessions = await _sessionService.GetAllSessionsAsync(CurrentUserId);
        return Success(
            sessions,
            "All sessions retrieved successfully.");
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> GetSessionsForUser(string userId)
    {
        var sessions = await _sessionService.GetAllSessionsAsync(userId);
        return Success(
            sessions,
            "Sessions for user retrieved successfully.");
    }

    [HttpDelete("{sessionId:guid}")]
    public async Task<IActionResult> RevokeSession(Guid sessionId)
    {
        await _sessionService.RevokeSessionAsync(sessionId, CurrentUserId);
        return NoContent();
    }

   
    [HttpDelete("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        await _sessionService.RevokeAllSessionsAsync(CurrentUserId);
        return NoContent();
    }
}