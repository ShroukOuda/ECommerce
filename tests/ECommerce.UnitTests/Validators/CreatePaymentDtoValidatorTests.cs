using ECommerce.Application.DTO.Payment;
using ECommerce.Application.Validators.Payment;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreatePaymentDtoValidatorTests
{
    private readonly CreatePaymentDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = TestGuid.FromInt(1), UserId = "user1", Amount = 99.99m, Method = "CreditCard", Currency = "USD"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithOrderIdZero_ShouldFail()
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = Guid.Empty, UserId = "user1", Amount = 99.99m, Method = "CreditCard", Currency = "USD"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_ShouldFail()
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = TestGuid.FromInt(1), UserId = "", Amount = 99.99m, Method = "CreditCard", Currency = "USD"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Fact]
    public async Task Validate_WithAmountZero_ShouldFail()
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = TestGuid.FromInt(1), UserId = "user1", Amount = 0, Method = "CreditCard", Currency = "USD"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Amount");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyMethod_ShouldFail(string? method)
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = TestGuid.FromInt(1), UserId = "user1", Amount = 99.99m, Method = method!, Currency = "USD"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Method");
    }

    [Fact]
    public async Task Validate_WithEmptyCurrency_ShouldFail()
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = TestGuid.FromInt(1), UserId = "user1", Amount = 99.99m, Method = "CreditCard", Currency = ""
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }

    [Fact]
    public async Task Validate_WithCurrencyTooLong_ShouldFail()
    {
        var dto = new CreatePaymentDTO
        {
            OrderId = TestGuid.FromInt(1), UserId = "user1", Amount = 99.99m, Method = "CreditCard", Currency = "TOOLONGCURR"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }
}
