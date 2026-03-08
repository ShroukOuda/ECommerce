using ECommerce.Application.DTO.ProductOption;
using ECommerce.Application.Validators.ProductOption;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateProductOptionDtoValidatorTests
{
    private readonly UpdateProductOptionDtoValidator _validator = new();

    private static UpdateProductOptionDTO CreateValidDto() => new()
    {
        Id = 1,
        Name = "Color",
        DisplayType = "Swatch",
        Type = "Visual",
        AttributeKey = "color"
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
        dto.Id = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Name = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameTooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Name = new string('A', 201);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyDisplayType_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.DisplayType = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyType_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Type = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptyAttributeKey_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.AttributeKey = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
