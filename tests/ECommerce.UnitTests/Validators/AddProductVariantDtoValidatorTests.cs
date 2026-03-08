using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Application.Validators.ProductVariant;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddProductVariantDtoValidatorTests
{
    private readonly AddProductVariantDtoValidator _validator = new();

    private static AddProductVariantDTO CreateValidDto() => new()
    {
        Sku = "SKU-001",
        VariantName = "Red Large",
        StockQuantity = 10,
        ProductId = 1,
        OptionValueIds = new List<int> { 1, 2 }
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
    public async Task Validate_WithEmptySku_ShouldFail(string? sku)
    {
        var dto = CreateValidDto();
        dto.Sku = sku!;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sku");
    }

    [Fact]
    public async Task Validate_WithSkuTooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Sku = new string('A', 101);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sku");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyVariantName_ShouldFail(string? name)
    {
        var dto = CreateValidDto();
        dto.VariantName = name!;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "VariantName");
    }

    [Fact]
    public async Task Validate_WithVariantNameTooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.VariantName = new string('A', 201);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "VariantName");
    }

    [Fact]
    public async Task Validate_WithNegativeStockQuantity_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.StockQuantity = -1;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    [Fact]
    public async Task Validate_WithZeroStockQuantity_ShouldPass()
    {
        var dto = CreateValidDto();
        dto.StockQuantity = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
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
