using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ECommerce.Infrastructure.Settings;
using ECommerce.Application.Interfaces.Services;


namespace ECommerce.Infrastructure.Services.Email;

public class SendGridEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SendGridEmailService(IOptions<EmailSettings> emailSettings)
        => _emailSettings = emailSettings.Value;

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default)
    {
        var client = new SendGridClient(_emailSettings.SendGridApiKey);
        var from = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
        var to = new EmailAddress(toEmail, toName);
        var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlBody);

        var response = await client.SendEmailAsync(message);

        if ((int)response.StatusCode >= 400)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid error {response.StatusCode}: {body}");
        }
    }
}