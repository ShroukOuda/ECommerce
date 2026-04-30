using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;


namespace ECommerce.Infrastructure.Services;

public class RequestContextService : IRequestContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;

        var ip = context?.Request.Headers["X-Forwarded-For"]
        .FirstOrDefault()
        ?.Split(',')[0].Trim();

        if (!string.IsNullOrEmpty(ip))
            return ip;

        return context?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    public string GetUserAgent()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.Request.Headers["User-Agent"].ToString() ?? "Unknown";
    }
}