using ECommerce.Application.DTO.ProductVariant;
using ECommerce.Application.Validators.ProductVariant;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateProductVariantDtoValidatorTests
{
    private readonly UpdateProductVariantDtoValidator _validator = new();

    private static UpdateProductVariantDTO CreateValidDto() => new()
    {
        Id = 1,
        Sku = "SKU-001",
        VariantName = "Red Large",
        StockQuantity = 10,
        Status = "Active"
    };

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = CreateValidDto();
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithIdZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Id = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
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
    public async Task Validate_WithNegativeStockQuantity_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.StockQuantity = -1;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyStatus_ShouldFail(string? status)
    {
        var dto = CreateValidDto();
        dto.Status = status!;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }
}
