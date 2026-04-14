using ECommerce.Application.DTO.Product;
using ECommerce.Application.Validators.Product;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddProductDtoValidatorTests
{
    private readonly AddProductDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddProductDTO { Name = "Laptop", Price = 999m, StockQuantity = 10, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyName_ShouldFail()
    {
        var dto = new AddProductDTO { Name = "", Price = 999m, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_WithZeroPrice_ShouldFail()
    {
        var dto = new AddProductDTO { Name = "Laptop", Price = 0, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public async Task Validate_WithNegativeStockQuantity_ShouldFail()
    {
        var dto = new AddProductDTO { Name = "Laptop", Price = 999m, StockQuantity = -1, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithEmptySKU_ShouldFail()
    {
        var dto = new AddProductDTO { Name = "Laptop", Price = 999m, SKU = "", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SKU");
    }

    [Fact]
    public async Task Validate_WithZeroCategoryId_ShouldFail()
    {
        var dto = new AddProductDTO { Name = "Laptop", Price = 999m, SKU = "LAP-001", CategoryId = Guid.Empty };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }
}
