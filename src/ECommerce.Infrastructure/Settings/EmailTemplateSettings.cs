namespace ECommerce.Infrastructure.Settings;

public class EmailTemplateSettings
{
    public const string SectionName = "EmailTemplateSettings";
    public string AppName { get; set; } = string.Empty;
    public string LogoText { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#1A56DB";
}