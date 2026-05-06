namespace ECommerce.Infrastructure.Settings;

public class EmailSettings
{
    public const string SectionName = "EmailS";

    public string Provider { get; set; } = string.Empty;
    public string SenderName { get; init; } = string.Empty;
    public string SenderEmail { get; init; } = string.Empty;
    public string SmtpHost { get; init; } = string.Empty;
    public string SmtpUser { get; init; } = string.Empty;
    public bool SmtpEnableSsl { get; init; }
    public int SmtpPort { get; init; } 
    public string SmtpPassword { get; init; } = string.Empty;
    public string SendGridApiKey { get; init; } = string.Empty;
    public string SupportEmail { get; init; } = string.Empty;
}