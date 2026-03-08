using ECommerce.Application.DTO.Cart;
using ECommerce.Application.Validators.Cart;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddCartItemDtoValidatorTests
{
    private readonly AddCartItemDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddCartItemDTO { CartId = 1, ProductId = 1, Quantity = 2 };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithCartIdZero_ShouldFail()
    {
        var dto = new AddCartItemDTO { CartId = 0, ProductId = 1, Quantity = 2 };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CartId");
    }

    [Fact]
    public async Task Validate_WithProductIdZero_ShouldFail()
    {
        var dto = new AddCartItemDTO { CartId = 1, ProductId = 0, Quantity = 2 };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public async Task Validate_WithQuantityZero_ShouldFail()
    {
        var dto = new AddCartItemDTO { CartId = 1, ProductId = 1, Quantity = 0 };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public async Task Validate_WithNegativeQuantity_ShouldFail()
    {
        var dto = new AddCartItemDTO { CartId = 1, ProductId = 1, Quantity = -1 };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
