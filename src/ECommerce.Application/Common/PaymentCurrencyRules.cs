using ECommerce.Domain.Exceptions;

namespace ECommerce.Application.Common;

/// <summary>Stripe card presentment rules, verified against https://docs.stripe.com/currencies.
/// Account and payment-method availability is still checked by Stripe.</summary>
public static class PaymentCurrencyRules
{
    private static readonly HashSet<string> Supported = new(
        ("USD AED AFN ALL AMD ANG AOA ARS AUD AWG AZN BAM BBD BDT BIF BMD BND BOB BRL BSD BWP BYN BZD CAD CDF CHF CLP CNY COP CRC CVE CZK DJF DKK DOP DZD EGP ETB EUR FJD FKP GBP GEL GIP GMD GNF GTQ GYD HKD HNL HTG HUF IDR ILS INR ISK JMD JPY KES KGS KHR KMF KRW KYD KZT LAK LBP LKR LRD LSL MAD MDL MGA MKD MMK MNT MOP MUR MVR MWK MXN MYR MZN NAD NGN NIO NOK NPR NZD PAB PEN PGK PHP PKR PLN PYG QAR RON RSD RUB RWF SAR SBD SCR SEK SGD SHP SLE SOS SRD STD SZL THB TJS TOP TRY TTD TWD TZS UAH UGX UYU UZS VND VUV WST XAF XCD XCG XOF XPF YER ZAR ZMW").Split(' '),
        StringComparer.Ordinal);

    private static readonly HashSet<string> ZeroDecimal = new(
        "BIF CLP DJF GNF JPY KMF KRW MGA PYG RWF VND VUV XAF XOF XPF".Split(' '),
        StringComparer.Ordinal);

    public static string Normalize(string? currency)
    {
        var normalized = currency?.Trim().ToUpperInvariant();
        if (normalized is null || !Supported.Contains(normalized))
            throw new BadRequestException("The configured or stored payment currency is not supported.");
        return normalized;
    }

    public static long ToMinorUnits(decimal amount, string currency)
    {
        var normalized = Normalize(currency);
        var wholeUnitsOnly = ZeroDecimal.Contains(normalized) || normalized is "ISK" or "UGX";
        var precision = wholeUnitsOnly ? 0 : 2;
        if (amount < 0 || decimal.Round(amount, precision) != amount)
            throw new BadRequestException($"{normalized} amounts must be nonnegative and have at most {precision} decimal places.");
        var factor = ZeroDecimal.Contains(normalized) ? 1m : 100m;
        try { return checked((long)(amount * factor)); }
        catch (OverflowException) { throw new BadRequestException("The order amount is too large."); }
    }
}
