using ECommerce.Infrastructure.Services.Payments;
using ECommerce.Infrastructure.Settings;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.UnitTests.Services;

public class CheckoutCurrencyProviderTests
{
    [Theory]
    [InlineData("usd")]
    [InlineData(" USD ")]
    [InlineData("uSd")]
    public void Currency_ReadsBoundStripeSettingsAndNormalizes(string configured)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Stripe:Currency"] = configured
        }).Build();
        var services = new ServiceCollection();
        services.Configure<StripeSettings>(config.GetSection(StripeSettings.SectionName));
        services.AddScoped<ICheckoutCurrencyProvider, CheckoutCurrencyProvider>();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<ICheckoutCurrencyProvider>().Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData("kwd")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Currency_RejectsInvalidOrUnsupportedConfiguration(string? configured)
    {
        var provider = new CheckoutCurrencyProvider(Options.Create(new StripeSettings { Currency = configured! }));
        var act = () => provider.Currency;
        act.Should().Throw<ECommerce.Domain.Exceptions.BadRequestException>();
    }
    [Theory]
    [InlineData("egp", "EGP")]
    [InlineData("eur", "EUR")]
    [InlineData("jpy", "JPY")]
    public void Currency_UsesChangedConfiguration(string configured, string expected)
    {
        var provider = new CheckoutCurrencyProvider(Options.Create(new StripeSettings { Currency = configured }));
        provider.Currency.Should().Be(expected);
    }
}
