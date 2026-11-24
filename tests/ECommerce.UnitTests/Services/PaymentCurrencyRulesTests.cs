using ECommerce.Application.Common;
using ECommerce.Domain.Exceptions;
using FluentAssertions;

namespace ECommerce.UnitTests.Services;

public class PaymentCurrencyRulesTests
{
    [Theory]
    [InlineData("USD", 12.34, 1234)]
    [InlineData("egp", 12.34, 1234)]
    [InlineData("EUR", 12.34, 1234)]
    [InlineData("JPY", 12, 12)]
    [InlineData("ISK", 12, 1200)]
    [InlineData("UGX", 12, 1200)]
    [InlineData("HUF", 12.34, 1234)]
    [InlineData("TWD", 12.34, 1234)]
    public void Conversion_UsesChargeMinorUnits(string currency, double amount, long expected)
        => PaymentCurrencyRules.ToMinorUnits((decimal)amount, currency).Should().Be(expected);

    [Theory]
    [InlineData("USD", 1.234)]
    [InlineData("JPY", 1.2)]
    [InlineData("ISK", 1.2)]
    [InlineData("UGX", 1.2)]
    [InlineData("EGP", -1)]
    public void Conversion_RejectsInvalidPrecisionWithoutRounding(string currency, double amount)
    {
        var act = () => PaymentCurrencyRules.ToMinorUnits((decimal)amount, currency);
        act.Should().Throw<BadRequestException>();
    }

    [Fact]
    public void Conversion_RejectsOverflow()
    {
        var act = () => PaymentCurrencyRules.ToMinorUnits(decimal.MaxValue, "USD");
        act.Should().Throw<BadRequestException>();
    }
}
