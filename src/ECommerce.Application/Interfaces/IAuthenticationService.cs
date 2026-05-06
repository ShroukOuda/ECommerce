using ECommerce.Application.DTO.Auth;

namespace ECommerce.Application.Interfaces;

public interface IAuthenticationService 
{
    public Task<RegisterResultDTO> RegisterAsync(RegisterDTO registerDto);
    public Task ConfirmEmailAsync(string userId, string token);
    public Task<AuthResultDTO> LoginAsync(LoginDTO loginDto);
    public Task<AuthResultDTO> RefreshTokenAsync(string refreshToken);
    public Task RevokeAsync(string refreshToken);
    public Task RevokeAllAsync(string userId);
    
}