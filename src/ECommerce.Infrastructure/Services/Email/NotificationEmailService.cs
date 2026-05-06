using ECommerce.Application.Interfaces.Email;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services.Email;
public sealed class NotificationEmailService : INotificationEmailService
{
    private readonly IEmailService _emailService;
    private readonly IUrlBuilder _urlBuilder;
    private readonly EmailTemplateBuilder _templateBuilder; 
    private readonly EmailTemplateSettings _settings;  

    public NotificationEmailService(
        IEmailService emailService,
        IUrlBuilder urlBuilder,
        EmailTemplateBuilder templateBuilder,
        IOptions<EmailTemplateSettings> options)   
    {
        _emailService = emailService;
        _urlBuilder = urlBuilder;
        _templateBuilder = templateBuilder;
        _settings = options.Value;
    }


    public async Task SendEmailConfirmationAsync(
        string toEmail,
        string toName,                          
        string confirmationLink,
        CancellationToken ct = default)
    {
        Validate(toEmail, toName);

        var body = $"""
            <h2>Confirm your email address</h2>
            <p>Hi {Safe(toName)},</p>
            <p>Thanks for creating your ECommerce account. Please confirm your email
               address to activate it.</p>
            {_templateBuilder.Button("Confirm email address", confirmationLink)}
            {_templateBuilder.Divider()}
            {_templateBuilder.Note("This link expires in 24 hours. If you didn't create an account, you can safely ignore this email.")}
            {_templateBuilder.FallbackLink(confirmationLink)}
            """;

        var html  = _templateBuilder.Wrap("Almost there — one quick step.", body);
        var plain = $"Hi {toName},\n\nConfirm your email: {confirmationLink}\n\nLink expires in 24 hours.";

        await _emailService.SendAsync(
            toEmail:   toEmail,
            toName:    toName,
            subject:   $"Confirm your { _settings.AppName } email address",
            htmlContent: html,
            ct: ct);
    }

    public async Task SendWelcomeEmailAsync(
        string toEmail,
        string toName,
        CancellationToken ct = default)
    {
        Validate(toEmail, toName);

        var body = $"""
            <h2>Welcome to ECommerce, {Safe(toName)}!</h2>
            <p>We're glad you're here. Here's what you can do right now:</p>
            <ul>
              <li>Browse thousands of products</li>
              <li>Save items to your wishlist</li>
              <li>Track your orders in real time</li>
            </ul>
            {_templateBuilder.Button("Start shopping", _urlBuilder.ProductList())}
            """;

       

        var html  = _templateBuilder.Wrap("Your account is ready.", body);
        var plain = $"Hi {toName},\n\nWelcome to ECommerce! Start shopping: {_urlBuilder.ProductList()}";

        await _emailService.SendAsync(
            toEmail: toEmail,
            toName:  toName,
            subject:  $"Welcome to {_settings.AppName}!",
            htmlContent: html,
            ct: ct);
    }


    public async Task SendSecurityAlertAsync(
        string toEmail,
        string toName,
        string ipAddress,
        string deviceInfo,
        string loginTime,
        string revokeAllLink,
        CancellationToken ct = default)
    {
        Validate(toEmail, toName);

        var body = $"""
            <h2>New sign-in detected</h2>
            <p>Hi {Safe(toName)},</p>
            <p>We noticed a new sign-in to your account:</p>
            <table style="border-collapse:collapse;width:100%;font-size:14px;">
              <tr><td style="padding:8px 0;color:#9ca3af;width:120px;">IP address</td>
                  <td style="padding:8px 0;font-weight:500;">{Safe(ipAddress)}</td></tr>
              <tr><td style="padding:8px 0;color:#9ca3af;">Device</td>
                  <td style="padding:8px 0;font-weight:500;">{Safe(deviceInfo)}</td></tr>
              <tr><td style="padding:8px 0;color:#9ca3af;">Time</td>
                  <td style="padding:8px 0;font-weight:500;">{Safe(loginTime)}</td></tr>
            </table>
            {_templateBuilder.Divider()}
            <p>If this was you, no action is needed.</p>
            <p>If you don't recognise this sign-in, log out of all devices immediately:</p>
            {_templateBuilder.Button("Log out everywhere", revokeAllLink)}
            """;

        var html  = _templateBuilder.Wrap("A new sign-in was detected on your account.", body);
        var plain = $"New sign-in to your {_settings.AppName} account.\nIP: {ipAddress}\nDevice: {deviceInfo}\nTime: {loginTime}\n\nNot you? Log out everywhere: {revokeAllLink}";

        await _emailService.SendAsync(
            toEmail:   toEmail,
            toName:    toName,
            subject:   "Security alert: new sign-in detected",
            htmlContent: html,
            ct: ct
        );
    }

    public async Task SendPasswordResetAsync(
        string toEmail,
        string toName,
        string resetLink,
        CancellationToken ct = default)
    {
        Validate(toEmail, toName);

        var body = $"""
            <h2>Reset your password</h2>
            <p>Hi {Safe(toName)},</p>
            <p>We received a request to reset your password. Click below to choose a new one.</p>
            {_templateBuilder.Button("Reset password", resetLink)}
            {_templateBuilder.Divider()}
            {_templateBuilder.Note("This link expires in 30 minutes. If you didn't request a reset, you can safely ignore this email.")}
            {_templateBuilder.FallbackLink(resetLink)}
            """;

        var html  = _templateBuilder.Wrap("Password reset requested.", body);

        await _emailService.SendAsync(
            toEmail: toEmail,
            toName: toName,
            subject: $"Reset your {_settings.AppName} password",
            htmlContent: html,
            ct: ct);
    }


    public async Task SendOrderConfirmationAsync(
        string  toEmail,
        string  toName,
        string  orderNumber,
        decimal totalAmount,
        string  currency,
        string  orderDetailsLink,
        CancellationToken ct = default)
    {
        Validate(toEmail, toName);

        var body = $"""
            <h2>Order confirmed</h2>
            <p>Hi {Safe(toName)},</p>
            <p>Thank you for your order! We've received it and will start processing it shortly.</p>
            <table style="border-collapse:collapse;width:100%;font-size:14px;">
              <tr><td style="padding:8px 0;color:#9ca3af;width:140px;">Order number</td>
                  <td style="padding:8px 0;font-weight:500;">#{Safe(orderNumber)}</td></tr>
              <tr><td style="padding:8px 0;color:#9ca3af;">Total</td>
                  <td style="padding:8px 0;font-weight:500;">{totalAmount:F2} {Safe(currency)}</td></tr>
            </table>
            {_templateBuilder.Button("View order", orderDetailsLink)}
            """;

        var html  = _templateBuilder.Wrap($"Order #{orderNumber} confirmed.", body);

        await _emailService.SendAsync(
            toEmail: toEmail,
            toName: toName,
            subject: $"Order #{orderNumber} confirmed",
            htmlContent: html,
            ct: ct);
    }

    public async Task SendOrderStatusUpdateAsync(
        string toEmail,
        string toName,
        string orderNumber,
        string oldStatus,
        string newStatus,
        string orderDetailsLink,
        CancellationToken ct = default)
    {
        Validate(toEmail, toName);

        var body = $"""
            <h2>Order update</h2>
            <p>Hi {Safe(toName)},</p>
            <p>Your order <strong>#{Safe(orderNumber)}</strong> status has changed:</p>
            <p>
              <span style="background:#e5e7eb;color:#6b7280;padding:4px 10px;border-radius:20px;font-size:13px;">{Safe(oldStatus)}</span>
              &nbsp;&rarr;&nbsp;
              <span style="background:#dcfce7;color:#15803d;padding:4px 10px;border-radius:20px;font-size:13px;font-weight:500;">{Safe(newStatus)}</span>
            </p>
            {_templateBuilder.Button("View order details", orderDetailsLink)}
            """;

        var html  = _templateBuilder.Wrap($"Order #{orderNumber} is now {newStatus}.", body);

        await _emailService.SendAsync(
            toEmail: toEmail,
            toName: toName,
            subject: $"Order #{orderNumber} is now {newStatus}",
            htmlContent: html,
            ct: ct);
    }


    private static void Validate(string email, string name)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email address is required.", nameof(email));

        if (!email.Contains('@'))
            throw new ArgumentException($"'{email}' is not a valid email address.", nameof(email));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Recipient name is required.", nameof(name));
    }

    private static string Safe(string? input) =>
        System.Net.WebUtility.HtmlEncode(input ?? string.Empty);
}