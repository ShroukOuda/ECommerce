using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Validators.Coupon;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddCouponDtoValidatorTests
{
    private readonly AddCouponDtoValidator _validator = new();

    private static AddCouponDTO CreateValidDto() => new()
    {
        Code = "SAVE20",
        Description = "Save 20%",
        DiscountType = "Percentage",
        DiscountValue = 20,
        MinPurchaseAmount = 50,
        MaxDiscountAmount = 100,
        UsageLimit = 100,
        PerUserLimit = 1,
        ValidFrom = DateTime.UtcNow,
        ValidUntil = DateTime.UtcNow.AddDays(30)
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
    public async Task Validate_WithEmptyCode_ShouldFail(string? code)
    {
        var dto = CreateValidDto();
        dto.Code = code!;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public async Task Validate_WithCodeTooLong_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Code = new string('A', 51);
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public async Task Validate_WithDiscountValueZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.DiscountValue = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiscountValue");
    }

    [Fact]
    public async Task Validate_WithNegativeMinPurchaseAmount_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.MinPurchaseAmount = -1;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinPurchaseAmount");
    }

    [Fact]
    public async Task Validate_WithNegativeMaxDiscountAmount_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.MaxDiscountAmount = -1;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MaxDiscountAmount");
    }

    [Fact]
    public async Task Validate_WithUsageLimitZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.UsageLimit = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UsageLimit");
    }

    [Fact]
    public async Task Validate_WithValidFromAfterValidUntil_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.ValidFrom = DateTime.UtcNow.AddDays(30);
        dto.ValidUntil = DateTime.UtcNow;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ValidFrom");
    }
}
