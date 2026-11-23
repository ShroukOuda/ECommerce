using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ECommerce.Domain.Exceptions;
using ECommerce.Infrastructure.Services.Payments;
using ECommerce.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace ECommerce.IntegrationTests;

public class StripeWebhookSignatureTests
{
    private const string TestSigningSecret = "whsec_isolated_signature_test";
    private readonly StripePaymentGatewayService _gateway = new(Options.Create(new StripeSettings { WebhookSecret = TestSigningSecret }));

    private static string Payload() => JsonSerializer.Serialize(new
    {
        id = "evt_isolated", @object = "event", type = "checkout.session.completed", api_version = "2025-03-31.basil",
        data = new { @object = new { id = "cs_isolated", @object = "checkout.session", payment_status = "paid", status = "complete", amount_total = 1234, currency = "usd",
            client_reference_id = "payment-id", metadata = new { payment_id = "payment-id", order_id = "order-id" } } }
    });

    private static string Sign(string payload, long timestamp)
    {
        var data = Encoding.UTF8.GetBytes($"{timestamp}.{payload}");
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(TestSigningSecret), data);
        return $"t={timestamp},v1={Convert.ToHexString(hash).ToLowerInvariant()}";
    }

    [Fact]
    public void ValidSignature_ParsesFinancialMetadataWithoutSecretApiKey()
    {
        var payload = Payload();
        var evt = _gateway.ParseWebhookEvent(payload, Sign(payload, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        evt.AmountTotal.Should().Be(1234);
        evt.PaymentStatus.Should().Be("paid");
        evt.Currency.Should().Be("usd");
        evt.PaymentId.Should().Be("payment-id");
        evt.OrderId.Should().Be("order-id");
    }

    [Theory]
    [InlineData("missing")][InlineData("wrong")][InlineData("expired")][InlineData("tampered")]
    public void InvalidSignature_IsRejected(string mode)
    {
        var payload = Payload();
        var signature = mode == "missing" ? "" : mode == "wrong" ? "t=1,v1=0000" : Sign(payload, DateTimeOffset.UtcNow.AddMinutes(mode == "expired" ? -20 : 0).ToUnixTimeSeconds());
        if (mode == "tampered") payload = payload.Replace("1234", "1235");
        FluentActions.Invoking(() => _gateway.ParseWebhookEvent(payload, signature)).Should().Throw<BadRequestException>();
    }
}
