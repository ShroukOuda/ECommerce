namespace ECommerce.Application.Interfaces.Email;

public interface IEmailService
{
    public Task SendAsync(string toEmail, string toName, string subject, string htmlContent);
   
}