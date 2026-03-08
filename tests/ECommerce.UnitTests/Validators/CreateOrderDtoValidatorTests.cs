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
            UserId = "user1",
            Currency = "USD",
            Items = new List<CreateOrderItemDTO> { new() { ProductId = 1, Quantity = 2 } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_ShouldFail()
    {
        var dto = new CreateOrderDTO
        {
            UserId = "",
            Currency = "USD",
            Items = new List<CreateOrderItemDTO> { new() { ProductId = 1, Quantity = 2 } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public async Task Validate_WithEmptyItems_ShouldFail()
    {
        var dto = new CreateOrderDTO { UserId = "user1", Currency = "USD", Items = new List<CreateOrderItemDTO>() };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }

    [Fact]
    public async Task Validate_WithEmptyCurrency_ShouldFail()
    {
        var dto = new CreateOrderDTO
        {
            UserId = "user1",
            Currency = "",
            Items = new List<CreateOrderItemDTO> { new() { ProductId = 1, Quantity = 2 } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }
}
