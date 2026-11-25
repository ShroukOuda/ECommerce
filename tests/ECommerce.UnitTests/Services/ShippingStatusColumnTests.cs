using ECommerce.Domain.Enums.Shipping;
using FluentAssertions;

namespace ECommerce.UnitTests.Services;

public class ShippingStatusColumnTests
{
    // Shippings.Status is stored as the enum name in nvarchar(20).
    [Fact]
    public void EveryShippingStatusName_FitsTheStatusColumn()
    {
        Enum.GetNames<ShippingStatus>().Should().OnlyContain(n => n.Length <= 20);
    }
}
