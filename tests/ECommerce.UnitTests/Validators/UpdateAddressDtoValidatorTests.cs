using ECommerce.Application.DTO.Address;
using ECommerce.Application.Validators.Address;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateAddressDtoValidatorTests
{
    private readonly UpdateAddressDtoValidator _validator = new();

    private static UpdateAddressDTO CreateValidDto() => new()
    {
        Id = TestGuid.FromInt(1),
        AddressLine1 = "123 Main St",
        City = "Springfield",
        State = "IL",
        PostalCode = "62701",
        Country = "US",
        Type = "Shipping"
    };

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = CreateValidDto();
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithIdZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Id = Guid.Empty;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithEmptyAddressLine1_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.AddressLine1 = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AddressLine1");
    }

    [Fact]
    public async Task Validate_WithEmptyCity_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.City = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
