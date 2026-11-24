using ECommerce.Application.DTO.Inventory;
using ECommerce.Application.Validators.Inventory;
using ECommerce.Domain.Enums.Inventory;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreateInventoryHistoryDtoValidatorTests
{
    private readonly CreateInventoryHistoryDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = TestGuid.FromInt(1), QuantityChange = 10, ChangeType = InventoryChangeType.Restock };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithoutProductVariantId_ShouldPassForSimpleProduct()
    {
        var dto = new CreateInventoryHistoryDTO { QuantityChange = -3, ChangeType = InventoryChangeType.Adjustment };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithUndefinedChangeType_ShouldFail()
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = TestGuid.FromInt(1), QuantityChange = 10, ChangeType = (InventoryChangeType)0 };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ChangeType");
    }

    [Theory]
    [InlineData(InventoryChangeType.Purchase)]
    [InlineData(InventoryChangeType.Restock)]
    [InlineData(InventoryChangeType.Adjustment)]
    [InlineData(InventoryChangeType.Return)]
    [InlineData(InventoryChangeType.Damage)]
    [InlineData(InventoryChangeType.Expired)]
    [InlineData(InventoryChangeType.Transfer)]
    public async Task Validate_WithDefinedChangeType_ShouldPass(InventoryChangeType changeType)
    {
        var dto = new CreateInventoryHistoryDTO { ProductVariantId = Guid.NewGuid(), QuantityChange = 1, ChangeType = changeType };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }
}
