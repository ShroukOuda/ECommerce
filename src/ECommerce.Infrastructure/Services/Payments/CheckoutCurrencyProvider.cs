using ECommerce.Application.Common;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services.Payments;

public sealed class CheckoutCurrencyProvider : ICheckoutCurrencyProvider
{
    private readonly IOptions<StripeSettings> _options;
    public CheckoutCurrencyProvider(IOptions<StripeSettings> options) => _options = options;
    public string Currency => PaymentCurrencyRules.Normalize(_options.Value.Currency);
}
