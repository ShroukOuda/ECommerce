using ECommerce.Application.DTO.Brand;
using ECommerce.Application.Validators.Brand;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateBrandDtoValidatorTests
{
    private readonly UpdateBrandDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new UpdateBrandDTO { Id = TestGuid.FromInt(1), Name = "Nike" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithZeroId_ShouldFail()
    {
        var dto = new UpdateBrandDTO { Id = Guid.Empty, Name = "Nike" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        var dto = new UpdateBrandDTO { Id = TestGuid.FromInt(1), Name = "" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithNameExceedingMaxLength_ShouldFail()
    {
        var dto = new UpdateBrandDTO { Id = TestGuid.FromInt(1), Name = new string('A', 101) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
