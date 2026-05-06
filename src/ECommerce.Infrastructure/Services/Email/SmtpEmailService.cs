using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ECommerce.Infrastructure.Settings;
using ECommerce.Application.Interfaces.Email;

namespace ECommerce.Infrastructure.Services.Email;
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings)
        => _emailSettings = emailSettings.Value;

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default)
    {
        using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
        {
            EnableSsl = _emailSettings.SmtpEnableSsl,
            Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPassword)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(new MailAddress(toEmail, toName));

        await client.SendMailAsync(message);
    }
}