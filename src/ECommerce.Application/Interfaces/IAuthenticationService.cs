using ECommerce.Application.DTO.Auth;

namespace ECommerce.Application.Interfaces;

public interface IAuthenticationService 
{
    public Task<RegisterResultDTO> RegisterAsync(RegisterDTO registerDto);
    public Task ConfirmEmailAsync(string email, string token);
    public Task<AuthResultDTO> LoginAsync(LoginDTO loginDto);
    public Task<AuthResultDTO> RefreshTokenAsync(string refreshToken);
    public Task ResendConfirmationEmailAsync(string email);
    public Task ForgotPasswordAsync(string email);
    public Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
    public Task RevokeAsync(string refreshToken);
    public Task RevokeAllAsync(string userId);
    
}