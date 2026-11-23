namespace ECommerce.Infrastructure.Settings;

public class StripeSettings
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string Currency { get; set; } = "usd";
    public string ClientUrl { get; set; } = "http://localhost:4200";
    public string SuccessPath { get; set; } = "/payment/success";
    public string CancelPath { get; set; } = "/payment/cancelled";
}