namespace ECommerce.Application.Interfaces.Services;

public interface INotificationEmailService
{
    Task SendEmailConfirmationAsync(
        string toEmail, 
        string toName, 
        string confirmationLink, 
        CancellationToken ct = default);
    Task SendWelcomeEmailAsync(
        string toEmail, 
        string toName, 
        CancellationToken ct = default);
    Task SendSecurityAlertAsync(
        string toEmail,
        string toName,
        string ipAddress,
        string deviceInfo,
        string loginTime,
        string revokeAllLink,
        CancellationToken ct = default);
 
    Task SendPasswordResetAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken ct = default);

    Task SendOrderConfirmationAsync(
        string  toEmail,
        string  toName,
        string  orderNumber,
        decimal totalAmount,
        string  currency,
        string  orderDetailsLink,
        CancellationToken ct = default);
 
    Task SendOrderStatusUpdateAsync(
        string toEmail,
        string toName,
        string orderNumber,
        string oldStatus,
        string newStatus,
        string orderDetailsLink,
        CancellationToken ct = default);

    
}