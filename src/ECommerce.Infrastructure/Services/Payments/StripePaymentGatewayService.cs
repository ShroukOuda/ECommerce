using System.Globalization;
using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Exceptions;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace ECommerce.Infrastructure.Services.Payments;

public class StripePaymentGatewayService : IPaymentGatewayService
{
    private readonly StripeSettings _settings;

    public StripePaymentGatewayService(IOptions<StripeSettings> options)
    {
        _settings = options.Value;

        if (!string.IsNullOrWhiteSpace(_settings.SecretKey))
        {
            StripeConfiguration.ApiKey = _settings.SecretKey;
        }
    }

    public async Task<CreateCheckoutSessionResponseDTO> CreateCheckoutSessionAsync(
        Payment payment,
        Order order,
        CancellationToken cancellationToken = default)
    {
        EnsureConfigured();

        var sessionService = new SessionService();
        var requestOptions = new RequestOptions
        {
            IdempotencyKey = payment.TransactionId
        };

        var successUrl = BuildClientUrl(_settings.ClientUrl, _settings.SuccessPath, payment.Id);
        var cancelUrl = BuildClientUrl(_settings.ClientUrl, _settings.CancelPath, payment.Id);

        var sessionOptions = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{successUrl}&session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{cancelUrl}&session_id={{CHECKOUT_SESSION_ID}}",
            ClientReferenceId = payment.Id.ToString(),
            Metadata = new Dictionary<string, string>
            {
                ["payment_id"] = payment.Id.ToString(),
                ["order_id"] = order.Id.ToString(),
                ["transaction_id"] = payment.TransactionId,
                ["currency"] = payment.Currency,
                ["amount_minor_units"] = payment.AmountInMinorUnits.ToString(CultureInfo.InvariantCulture)
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = payment.Currency.ToLowerInvariant(),
                        UnitAmount = payment.AmountInMinorUnits,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Order {order.OrderNumber}",
                            Description = $"Payment for order {order.OrderNumber}"
                        }
                    }
                }
            },
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    ["payment_id"] = payment.Id.ToString(),
                    ["order_id"] = order.Id.ToString(),
                    ["transaction_id"] = payment.TransactionId
                }
            }
        };

        var session = await sessionService.CreateAsync(sessionOptions, requestOptions);

        return new CreateCheckoutSessionResponseDTO
        {
            PaymentId = payment.Id,
            OrderId = order.Id,
            TransactionId = payment.TransactionId,
            CheckoutSessionId = session.Id,
            CheckoutSessionUrl = session.Url,
            PaymentIntentId = session.PaymentIntentId,
            Amount = payment.Amount,
            AmountInMinorUnits = payment.AmountInMinorUnits,
            Currency = payment.Currency,
            Status = session.Status,
            CheckoutSessionStatus = session.Status
        };
    }

    public StripeWebhookEventDTO ParseWebhookEvent(string payload, string signatureHeader)
    {
        EnsureConfigured();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signatureHeader,
                _settings.WebhookSecret,
                throwOnApiVersionMismatch: false);

            var checkoutSession = stripeEvent.Data.Object as Session;

            return new StripeWebhookEventDTO
            {
                EventId = stripeEvent.Id,
                EventType = stripeEvent.Type,
                CheckoutSessionId = checkoutSession?.Id,
                PaymentIntentId = checkoutSession?.PaymentIntentId,
                PaymentStatus = checkoutSession?.PaymentStatus,
                CheckoutSessionStatus = checkoutSession?.Status,
                ClientReferenceId = checkoutSession?.ClientReferenceId,
                AmountTotal = checkoutSession?.AmountTotal,
                Currency = checkoutSession?.Currency,
                PayloadSummary = $"Stripe event {stripeEvent.Type} for session {checkoutSession?.Id ?? "unknown"}"
            };
        }
        catch (StripeException ex)
        {
            throw new BadRequestException("Invalid Stripe webhook signature.", ex);
        }
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_settings.SecretKey))
        {
            throw new InvalidOperationException("Stripe secret key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret))
        {
            throw new InvalidOperationException("Stripe webhook secret is not configured.");
        }
    }

    private static string BuildClientUrl(string clientUrl, string path, Guid paymentId)
    {
        var normalizedBaseUrl = clientUrl.TrimEnd('/');
        var normalizedPath = path.StartsWith('/') ? path : $"/{path}";
        return $"{normalizedBaseUrl}{normalizedPath}?paymentId={paymentId}";
    }
}