using ECommerce.Domain.Entities.User;

namespace ECommerce.Application.Interfaces.Services;

public interface IIdentityService
{
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUsernameAsync(string username);
    Task<User?> FindByPhoneNumberAsync(string phoneNumber);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<(bool Success, string[] Errors)> CreateUserAsync(User user, string password);
    Task AddToRoleAsync(User user, string role);
    Task<IList<string>> GetRolesAsync(User user);
    Task<bool> ConfirmEmailAsync(User user, string token);  
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<bool> ResetPasswordAsync(User user, string token, string newPassword);
}