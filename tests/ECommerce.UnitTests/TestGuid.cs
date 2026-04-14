namespace ECommerce.UnitTests;

public static class TestGuid
{
    public static Guid FromInt(int value)
    {
        return Guid.Parse($"00000000-0000-0000-0000-{value:D12}");
    }

    public static Guid? FromNullableInt(int? value)
    {
        return value.HasValue ? FromInt(value.Value) : null;
    }
}
