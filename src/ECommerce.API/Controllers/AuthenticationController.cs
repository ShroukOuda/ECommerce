using ECommerce.Application.DTO.Auth;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;


public class AuthenticationController : BaseController
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService)
        => _authService = authService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(result);    
    }


    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string userId,
        [FromQuery] string token)
    {
        await _authService.ConfirmEmailAsync(userId, token);
        return Ok(new ResponseAPI(200, "Email confirmed successfully"));
    }

    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);

    }

   
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await _authService.RefreshTokenAsync(refreshToken);
        return Ok(result);
    }

    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        await _authService.RevokeAsync(refreshToken);
        return NoContent();
    }

   
    [HttpPost("logout-all")]
    [Authorize]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
        await _authService.RevokeAllAsync(userId);
        return NoContent();
    }
}