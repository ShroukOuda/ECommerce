using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ECommerce.Domain.Entities.User;
using ECommerce.Domain.Enums.User;
using ECommerce.Infrastructure.Settings;

namespace ECommerce.Infrastructure.Persistence.Seed.Identity;
public class AdminSeeder
{
    private readonly UserManager<User> _userManager;

    private readonly AdminSeedSettings _settings;


    public AdminSeeder(
        UserManager<User> userManager,
        IOptions<AdminSeedSettings> settings)
    {
        _userManager = userManager;
        _settings = settings.Value;
    }

    public async Task SeedAsync()
    {
        var user = await _userManager.FindByNameAsync(_settings.UserName);

        if (user != null)
        {
            await EnsureRoleAsync(user, AppRoles.Admin.ToString());
            return;
        }

        var admin = new User
        {
            UserName = _settings.UserName,
            CountryCode = _settings.CountryCode,
            PhoneNumber = _settings.PhoneNumber,
            Email = _settings.Email,
            FirstName = _settings.FirstName,
            LastName = _settings.LastName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, _settings.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));

        await EnsureRoleAsync(admin, AppRoles.Admin.ToString());
    }

    private async Task EnsureRoleAsync(User user, string role)
    {
        if (await _userManager.IsInRoleAsync(user, role))
            return;

        await _userManager.AddToRoleAsync(user, role);
    }
}