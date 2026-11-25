using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ECommerce.Infrastructure.Settings;
using ECommerce.Application.Interfaces.Email;

namespace ECommerce.Infrastructure.Services.Email;
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
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

        try
        {
            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            // Diagnostic only: exception chain plus non-secret connection settings.
            // Never logs credentials, recipient, subject or body. The exception is rethrown unchanged.
            _logger.LogError(
                "SMTP send failed. Host={Host} Port={Port} EnableSsl={EnableSsl}. Exception chain: {Chain}",
                _emailSettings.SmtpHost,
                _emailSettings.SmtpPort,
                _emailSettings.SmtpEnableSsl,
                DescribeChain(ex));
            throw;
        }
    }

    private static string DescribeChain(Exception? ex)
    {
        var parts = new List<string>();
        for (var depth = 0; ex is not null && depth < 8; depth++, ex = ex.InnerException)
        {
            var status = ex is SmtpException smtp ? $" [SmtpStatusCode={smtp.StatusCode}]" : string.Empty;
            parts.Add($"{ex.GetType().FullName}{status}: {ex.Message}");
        }
        return string.Join(" -> ", parts);
    }
}
