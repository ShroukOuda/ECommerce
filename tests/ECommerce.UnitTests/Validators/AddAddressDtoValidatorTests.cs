using ECommerce.Application.DTO.Address;
using ECommerce.Application.Validators.Address;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddAddressDtoValidatorTests
{
    private readonly AddAddressDtoValidator _validator = new();

    private static AddAddressDTO CreateValidDto() => new()
    {
        UserId = "user1",
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyUserId_ShouldFail(string? userId)
    {
        var dto = CreateValidDto();
        dto.UserId = userId!;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyAddressLine1_ShouldFail(string? line1)
    {
        var dto = CreateValidDto();
        dto.AddressLine1 = line1!;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AddressLine1");
    }

    [Fact]
    public async Task Validate_WithAddressLine1TooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.AddressLine1 = new string('A', 201);
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
        result.Errors.Should().Contain(e => e.PropertyName == "City");
    }

    [Fact]
    public async Task Validate_WithCityTooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.City = new string('A', 101);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "City");
    }

    [Fact]
    public async Task Validate_WithEmptyState_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.State = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "State");
    }

    [Fact]
    public async Task Validate_WithEmptyPostalCode_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.PostalCode = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PostalCode");
    }

    [Fact]
    public async Task Validate_WithEmptyCountry_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Country = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Country");
    }

    [Fact]
    public async Task Validate_WithEmptyType_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Type = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }
}
