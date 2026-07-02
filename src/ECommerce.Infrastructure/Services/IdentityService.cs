using ECommerce.Application.Interfaces;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public IdentityService(UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User?> FindByIdAsync(string userId)
        => await _userManager.FindByIdAsync(userId);
    public async Task<User?> FindByEmailAsync(string email)
        => await _userManager.FindByEmailAsync(email);

    public async Task<User?> FindByUsernameAsync(string username)
        => await _userManager.FindByNameAsync(username);

    public async Task<User?> FindByPhoneNumberAsync(string phoneNumber)
        => await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

    public async Task<bool> CheckPasswordAsync(User user, string password)
        => (await _signInManager.CheckPasswordSignInAsync(user, password, false)).Succeeded;

    public async Task<(bool Success, string[] Errors)> CreateUserAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task AddToRoleAsync(User user, string role)
        => await _userManager.AddToRoleAsync(user, role);

    public async Task<IList<string>> GetRolesAsync(User user)
        => await _userManager.GetRolesAsync(user);

    public async Task<bool> ConfirmEmailAsync(User user, string token)
    {      
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;    
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
    {
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }
}