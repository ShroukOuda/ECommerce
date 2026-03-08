using ECommerce.Application.DTO.Category;
using ECommerce.Application.Validators.Category;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddCategoryDtoValidatorTests
{
    private readonly AddCategoryDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddCategoryDTO { Name = "Electronics", Description = "Devices" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        var dto = new AddCategoryDTO { Name = "", Description = "Devices" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameExceedingMaxLength_ShouldFail()
    {
        var dto = new AddCategoryDTO { Name = new string('A', 101) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithDescriptionExceedingMaxLength_ShouldFail()
    {
        var dto = new AddCategoryDTO { Name = "Electronics", Description = new string('A', 501) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
