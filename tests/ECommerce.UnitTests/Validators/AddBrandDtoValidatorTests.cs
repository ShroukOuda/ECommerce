using ECommerce.Application.DTO.Brand;
using ECommerce.Application.Validators.Brand;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddBrandDtoValidatorTests
{
    private readonly AddBrandDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddBrandDTO { Name = "Nike" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        var dto = new AddBrandDTO { Name = "" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithInvalidName_ShouldFail(string? name)
    {
        var dto = new AddBrandDTO { Name = name! };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
