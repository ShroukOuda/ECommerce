using ECommerce.Application.DTO.Shipping;
using ECommerce.Application.Validators.Shipping;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreateShippingDtoValidatorTests
{
    private readonly CreateShippingDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new CreateShippingDTO { OrderId = 1, AddressId = 1, Cost = 9.99m, Method = "Express" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithOrderIdZero_ShouldFail()
    {
        var dto = new CreateShippingDTO { OrderId = 0, AddressId = 1, Cost = 9.99m, Method = "Express" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public async Task Validate_WithAddressIdZero_ShouldFail()
    {
        var dto = new CreateShippingDTO { OrderId = 1, AddressId = 0, Cost = 9.99m, Method = "Express" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AddressId");
    }

    [Fact]
    public async Task Validate_WithNegativeCost_ShouldFail()
    {
        var dto = new CreateShippingDTO { OrderId = 1, AddressId = 1, Cost = -1, Method = "Express" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cost");
    }

    [Fact]
    public async Task Validate_WithZeroCost_ShouldPass()
    {
        var dto = new CreateShippingDTO { OrderId = 1, AddressId = 1, Cost = 0, Method = "Express" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyMethod_ShouldFail(string? method)
    {
        var dto = new CreateShippingDTO { OrderId = 1, AddressId = 1, Cost = 9.99m, Method = method! };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Method");
    }
}
