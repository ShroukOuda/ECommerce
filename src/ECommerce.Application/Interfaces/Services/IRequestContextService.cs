namespace ECommerce.Application.Interfaces.Services;

public interface IRequestContextService
{
    public string? GetUserId();
    public string? GetIpAddress();
    public string? GetUserAgent();
}