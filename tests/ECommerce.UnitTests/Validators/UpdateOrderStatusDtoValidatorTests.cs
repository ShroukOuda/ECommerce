using ECommerce.Application.DTO.Order;
using ECommerce.Application.Validators.Order;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateOrderStatusDtoValidatorTests
{
    private readonly UpdateOrderStatusDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new UpdateOrderStatusDTO { Id = 1, OrderStatus = "Shipped" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithIdZero_ShouldFail()
    {
        var dto = new UpdateOrderStatusDTO { Id = 0, OrderStatus = "Shipped" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithNegativeId_ShouldFail()
    {
        var dto = new UpdateOrderStatusDTO { Id = -1, OrderStatus = "Shipped" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyOrderStatus_ShouldFail(string? status)
    {
        var dto = new UpdateOrderStatusDTO { Id = 1, OrderStatus = status! };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderStatus");
    }
}
