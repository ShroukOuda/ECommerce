namespace ECommerce.Application.Interfaces.Services;

public interface IRequestContextService
{
    string GetIpAddress();
    string GetUserAgent();
}