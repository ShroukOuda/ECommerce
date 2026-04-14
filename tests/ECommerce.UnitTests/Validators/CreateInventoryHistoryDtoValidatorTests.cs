using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Validators.Inventory;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreateInventoryHistoryDtoValidatorTests
{
    private readonly CreateInventoryHistoryDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = TestGuid.FromInt(1), NewQuantity = 10, ChangeType = "Addition" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithProductIdZero_ShouldFail()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = Guid.Empty, NewQuantity = 10, ChangeType = "Addition" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public async Task Validate_WithNegativeNewQuantity_ShouldFail()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = TestGuid.FromInt(1), NewQuantity = -1, ChangeType = "Addition" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewQuantity");
    }

    [Fact]
    public async Task Validate_WithZeroNewQuantity_ShouldPass()
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = TestGuid.FromInt(1), NewQuantity = 0, ChangeType = "Addition" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyChangeType_ShouldFail(string? changeType)
    {
        var dto = new CreateInventoryHistoryDTO { ProductId = TestGuid.FromInt(1), NewQuantity = 10, ChangeType = changeType! };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChangeType");
    }
}
