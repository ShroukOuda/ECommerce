using ECommerce.Application.DTO.ProductOption;
using ECommerce.Application.Validators.ProductOption;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddProductOptionDtoValidatorTests
{
    private readonly AddProductOptionDtoValidator _validator = new();

    private static AddProductOptionDTO CreateValidDto() => new()
    {
        Name = "Color",
        DisplayType = "Swatch",
        Type = "Visual",
        AttributeKey = "color",
        ProductId = 1
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
    public async Task Validate_WithEmptyName_ShouldFail(string? name)
    {
        var dto = CreateValidDto();
        dto.Name = name!;
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
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithEmptyDisplayType_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.DisplayType = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DisplayType");
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

    [Fact]
    public async Task Validate_WithEmptyAttributeKey_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.AttributeKey = "";
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AttributeKey");
    }

    [Fact]
    public async Task Validate_WithAttributeKeyTooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.AttributeKey = new string('A', 101);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AttributeKey");
    }

    [Fact]
    public async Task Validate_WithProductIdZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.ProductId = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }
}
