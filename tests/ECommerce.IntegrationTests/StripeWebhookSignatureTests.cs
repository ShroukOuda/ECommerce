using Microsoft.EntityFrameworkCore;
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

    [Theory]
    [InlineData("checkout.session.completed")]
    [InlineData("checkout.session.async_payment_succeeded")]
    public async Task SignedLateSuccess_PersistsPaidCancelledAndReviewAcrossReload(string eventType)
    {
        // Unique InMemory test data only. No SQL connection or schema-creation call.
        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<ECommerce.Infrastructure.Persistence.Context.AppDbContext>()
            .UseInMemoryDatabase("late-payment-" + Guid.NewGuid()).Options;
        var orderId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        await using (var context = new ECommerce.Infrastructure.Persistence.Context.AppDbContext(options))
        {
            context.Set<ECommerce.Domain.Entities.Orders.Order>().Add(new()
            { Id = orderId, UserId = "isolated-owner", OrderStatus = ECommerce.Domain.Enums.Order.OrderStatus.Cancelled });
            context.Set<ECommerce.Domain.Entities.Payments.Payment>().Add(new()
            { Id = paymentId, OrderId = orderId, UserId = "isolated-owner", Currency = "USD", Amount = 12.34m,
                AmountInMinorUnits = 1234, Status = ECommerce.Domain.Enums.Payment.PaymentStatus.Expired,
                StripeCheckoutSessionId = "cs_isolated", StripePaymentIntentId = "pi_isolated" });
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
            var payload = JsonSerializer.Serialize(new
            {
                id = "evt_late_isolated", @object = "event", type = eventType, api_version = "2025-03-31.basil",
                data = new { @object = new { id = "cs_isolated", @object = "checkout.session", payment_intent = "pi_isolated",
                    payment_status = "paid", status = "complete", amount_total = 1234, currency = "usd",
                    client_reference_id = paymentId.ToString(), metadata = new { payment_id = paymentId.ToString(), order_id = orderId.ToString() } } }
            });
            var service = new ECommerce.Application.Services.PaymentService(new ECommerce.Infrastructure.Repositories.UnitOfWork(context),
                null!, null!, _gateway);
            var signature = Sign(payload, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            await service.HandleStripeWebhookAsync(payload, signature);
            // Duplicate event does not create another review record or repeat a transition.
            await service.HandleStripeWebhookAsync(payload, signature);
        }
        await using var reloaded = new ECommerce.Infrastructure.Persistence.Context.AppDbContext(options);
        var payment = await reloaded.Set<ECommerce.Domain.Entities.Payments.Payment>().SingleAsync();
        payment.Status.Should().Be(ECommerce.Domain.Enums.Payment.PaymentStatus.Paid);
        payment.PaidAt.Should().NotBeNull();
        (await reloaded.Set<ECommerce.Domain.Entities.Orders.Order>().SingleAsync()).OrderStatus
            .Should().Be(ECommerce.Domain.Enums.Order.OrderStatus.Cancelled);
        (await reloaded.Set<ECommerce.Domain.Entities.Payments.StripeWebhookEvent>().SingleAsync()).ProcessingResult
            .Should().Be("ManualReviewRequired");
        (await reloaded.Set<ECommerce.Domain.Entities.Orders.OrderStatusHistory>().CountAsync()).Should().Be(0);
    }
}
