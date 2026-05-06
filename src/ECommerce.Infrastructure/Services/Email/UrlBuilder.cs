using ECommerce.Application.Interfaces.Email;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services.Email;

public class UrlBuilder : IUrlBuilder
{
    private readonly AppSettings _appSettings;
 
    public UrlBuilder(IOptions<AppSettings> appSettings)
        => _appSettings = appSettings.Value;
 
    public string EmailConfirmation(string userId, string rawToken) =>
        Build("/api/Authentication/confirm-email",
            ("userId", userId),
            ("token", rawToken));   
 
    public string PasswordReset(string userId, string rawToken) =>
        Build("/api/Authentication/reset-password",
            ("userId", userId),
            ("token", rawToken));
 
    public string OrderDetails(string orderId) =>
        Build($"/orders/{Uri.EscapeDataString(orderId)}");
 
    public string RevokeAllSessions() =>
        Build("/api/Authentication/logout-all");
 
    public string ProductList() =>
        Build("/products");
 
 
    private string Build(string path, params (string key, string value)[] query)
    {
        var baseUrl = _appSettings.AppBaseUrl.TrimEnd('/');
        if (query.Length == 0) return $"{baseUrl}{path}";
 
        var qs = string.Join("&",
            query.Select(q =>
                $"{Uri.EscapeDataString(q.key)}={Uri.EscapeDataString(q.value)}"));
 
        return $"{baseUrl}{path}?{qs}";
    }
}