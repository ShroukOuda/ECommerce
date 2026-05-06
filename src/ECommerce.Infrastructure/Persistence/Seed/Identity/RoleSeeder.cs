using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ECommerce.Domain.Enums.User;

namespace ECommerce.Infrastructure.Persistence.Seed.Identity;


public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(RoleManager<IdentityRole> roleManager, ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        foreach (var roleName in Enum.GetNames(typeof(AppRoles)))
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                continue;

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
                throw new Exception($"Role {roleName} failed: " +
                    string.Join(",", result.Errors.Select(e => e.Description)));

            _logger.LogInformation("Role created: {Role}", roleName);
        }
    }
}