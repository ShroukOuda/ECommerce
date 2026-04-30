using ECommerce.Application.DTO.Authentication;

namespace ECommerce.Application.Interfaces;

public interface IAuthenticationService 
{
    public Task<UserResultDto> RegisterAsync(RegisterDTO registerDto);
    public Task<UserResultDto> LoginAsync(LoginDTO loginDto);
    public Task LogoutAsync(string userId, Guid sessionId);
    
}