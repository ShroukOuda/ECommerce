using ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Interfaces.Services;

public interface ITokenService
{
    public Task<string> GenerateTokenAsync(User user);
    public Task<string> GenerateRefreshTokenAsync();
}