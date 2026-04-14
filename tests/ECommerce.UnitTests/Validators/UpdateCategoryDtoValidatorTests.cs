using ECommerce.Application.DTO.Category;
using ECommerce.Application.Validators.Category;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateCategoryDtoValidatorTests
{
    private readonly UpdateCategoryDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new UpdateCategoryDTO { Id = TestGuid.FromInt(1), Name = "Electronics", Description = "Devices" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithZeroId_ShouldFail()
    {
        var dto = new UpdateCategoryDTO { Id = Guid.Empty, Name = "Electronics" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        var dto = new UpdateCategoryDTO { Id = TestGuid.FromInt(1), Name = "" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
