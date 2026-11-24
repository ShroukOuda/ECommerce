namespace ECommerce.Application.Interfaces.Services;

/// <summary>Provides the server-configured catalog currency for new orders.</summary>
public interface ICheckoutCurrencyProvider
{
    string Currency { get; }
}
