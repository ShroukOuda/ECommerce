using ECommerce.Application.DTO.ProductOption;
using ECommerce.Application.Validators.ProductOption;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddProductOptionValueDtoValidatorTests
{
    private readonly AddProductOptionValueDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddProductOptionValueDTO { Value = "Red", Label = "Red Color" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyValue_ShouldFail(string? value)
    {
        var dto = new AddProductOptionValueDTO { Value = value!, Label = "Red Color" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Value");
    }

    [Fact]
    public async Task Validate_WithValueTooLong_ShouldFail()
    {
        var dto = new AddProductOptionValueDTO { Value = new string('A', 201), Label = "Red" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Value");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyLabel_ShouldFail(string? label)
    {
        var dto = new AddProductOptionValueDTO { Value = "Red", Label = label! };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Label");
    }

    [Fact]
    public async Task Validate_WithLabelTooLong_ShouldFail()
    {
        var dto = new AddProductOptionValueDTO { Value = "Red", Label = new string('A', 201) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Label");
    }

}
