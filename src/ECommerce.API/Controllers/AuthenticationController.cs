using ECommerce.Application.DTO.Auth;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Controllers;


[Route("api/v1/auth")]
public class AuthenticationController : BaseController
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService)
        => _authService = authService;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        await _authService.RegisterAsync(dto);
        return CreatedMessage("Registration successful. Please check your email to confirm your account.");
    }


    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string email,
        [FromQuery] string token)
    {
        await _authService.ConfirmEmailAsync(email, token);
        return SuccessMessage("Email confirmed successfully. You can now log in.");
    }

    
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Success(result, "Login successful.");

    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        await _authService.ForgotPasswordAsync(email);
        return SuccessMessage("Password reset email sent successfully. Please check your email.");
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        await _authService.ResetPasswordAsync(dto);
        return SuccessMessage("Password reset successfully. You can now log in with your new password.");
    }

    [HttpPost("resend-confirmation-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] string email)
    {
        await _authService.ResendConfirmationEmailAsync(email);
        return SuccessMessage("Confirmation email resent successfully. Please check your email.");
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await _authService.RefreshTokenAsync(refreshToken);
        return Success(result, "Token refreshed successfully.");
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
        var userId = CurrentUserId;
        await _authService.RevokeAllAsync(userId);
        return NoContent();
    }
}