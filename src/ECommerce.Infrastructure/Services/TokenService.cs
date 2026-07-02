using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly UserManager<User> _userManager;

    public TokenService(
        IOptions<JwtOptions> jwtOptions,
        UserManager<User> userManager)
    {
        _jwtOptions = jwtOptions;
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var jwtOptions = _jwtOptions.Value;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("createdAt", user.CreatedAt.ToString("o")),
            new Claim("updatedAt", user.UpdatedAt.ToString("o")),
            new Claim(ClaimTypes.Name, user.FirstName ?? string.Empty),
            new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty)
        };
        
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)        
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public Task<string> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Task.FromResult(Convert.ToBase64String(randomNumber));
    }

}