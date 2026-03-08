using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Validators.Coupon;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class UpdateCouponDtoValidatorTests
{
    private readonly UpdateCouponDtoValidator _validator = new();

    private static UpdateCouponDTO CreateValidDto() => new()
    {
        Id = 1,
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

    [Fact]
    public async Task Validate_WithIdZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Id = 0;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task Validate_WithEmptyCode_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Code = "";
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
    }
}
