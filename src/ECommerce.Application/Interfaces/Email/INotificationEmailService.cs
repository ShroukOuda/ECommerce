namespace ECommerce.Application.Interfaces.Email;

public interface INotificationEmailService
{
    Task SendEmailConfirmationAsync(
        string toEmail, 
        string toName, 
        string confirmationLink);
    Task SendWelcomeEmailAsync(
        string toEmail, 
        string toName);
    Task SendSecurityAlertAsync(
        string toEmail,
        string toName,
        string ipAddress,
        string deviceInfo,
        string loginTime,
        string revokeAllLink);
 
    Task SendPasswordResetAsync(
        string toEmail,
        string toName,
        string resetLink);

    Task SendOrderConfirmationAsync(
        string  toEmail,
        string  toName,
        string  orderNumber,
        decimal totalAmount,
        string  currency,
        string  orderDetailsLink);
 
    Task SendOrderStatusUpdateAsync(
        string toEmail,
        string toName,
        string orderNumber,
        string oldStatus,
        string newStatus,
        string orderDetailsLink);

    
}