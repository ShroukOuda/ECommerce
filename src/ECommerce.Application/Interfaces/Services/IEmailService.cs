namespace ECommerce.Application.Interfaces.Services;

public interface IEmailService
{
    public Task SendAsync(string toEmail, string toName, string subject, string htmlContent, CancellationToken ct = default);
   
}