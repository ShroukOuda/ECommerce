using ECommerce.Application.DTO.UserSession;
using ECommerce.Application.Interfaces;

namespace ECommerce.API.Controllers;

public class UserSessionsController : BaseController
{
    private readonly IUserSessionService _userSessionService;

    public UserSessionsController(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var sessions = await _userSessionService.GetSessionsByUserIdAsync(userId);
        return Ok(sessions);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userSessionService.DeleteSessionAsync(id);
        return Ok(new ResponseAPI(200, "Session deleted successfully"));
    }
}
