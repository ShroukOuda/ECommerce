using ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByPhoneNumberAsync(string phoneNumber);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<(bool Success, string[] Errors)> CreateUserAsync(User user, string password);
    Task AddToRoleAsync(User user, string role);
}