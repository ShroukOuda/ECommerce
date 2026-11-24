namespace ECommerce.Domain.Exceptions;

/// <summary>A client operation rejected by a known business rule, not an infrastructure failure.</summary>
public sealed class BusinessRuleException : InvalidOperationException
{
    public BusinessRuleException(string message) : base(message) { }
}
