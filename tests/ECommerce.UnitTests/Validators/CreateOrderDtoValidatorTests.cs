using ECommerce.Application.DTO.Order;
using ECommerce.Application.Validators.Order;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreateOrderDtoValidatorTests
{
    private readonly CreateOrderDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new CreateOrderDTO
        {
            CartId = TestGuid.FromInt(100),
            Currency = "USD",
            Items = new List<CreateOrderItemDTO> { new() { ProductId = TestGuid.FromInt(1), ProductVariantId = TestGuid.FromInt(2), Quantity = 2 } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }


    [Fact]
    public async Task Validate_WithEmptyCartId_ShouldFail()
    {
        var dto = new CreateOrderDTO
        {
            Currency = "USD",
            Items = new List<CreateOrderItemDTO> { new() { ProductId = TestGuid.FromInt(1), ProductVariantId = TestGuid.FromInt(2), Quantity = 2 } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CartId");
    }

    [Fact]
    public async Task Validate_WithEmptyCurrency_ShouldPassBecauseCurrencyIsServerOwned()
    {
        var dto = new CreateOrderDTO
        {

            CartId = TestGuid.FromInt(100),
            Currency = "",
            Items = new List<CreateOrderItemDTO> { new() { ProductId = TestGuid.FromInt(1), Quantity = 2 } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }
}
