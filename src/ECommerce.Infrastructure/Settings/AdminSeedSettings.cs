namespace ECommerce.Infrastructure.Settings;

public class AdminSeedSettings
{
    public const string SectionName = "AdminSeed";
 
    public string UserName { get; init; } = "admin";
    public string CountryCode { get; init; } = string.Empty;
    public string FirstName { get; init; } = "System";
    public string LastName { get; init; } = "Admin";
    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}