using ECommerce.Application.DTO.Product;
using ECommerce.Application.Validators.Product;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateProductDtoValidatorTests
{
    private readonly UpdateProductDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new UpdateProductDTO { Id = TestGuid.FromInt(1), Name = "Laptop", BasePrice = 999m, StockQuantity = 10, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithZeroId_ShouldFail()
    {
        var dto = new UpdateProductDTO { Id = Guid.Empty, Name = "Laptop", BasePrice = 999m, SKU = "LAP-001", CategoryId = TestGuid.FromInt(1) };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithZeroCategoryId_ShouldFail()
    {
        var dto = new UpdateProductDTO { Id = TestGuid.FromInt(1), Name = "Laptop", BasePrice = 999m, SKU = "LAP-001", CategoryId = Guid.Empty };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }
}
