using ECommerce.Application.DTO.Wishlist;
using ECommerce.Application.Validators.Wishlist;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddWishlistDtoValidatorTests
{
    private readonly AddWishlistDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddWishlistDTO { ProductId = 1, UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithProductIdZero_ShouldFail()
    {
        var dto = new AddWishlistDTO { ProductId = 0, UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyUserId_ShouldFail(string? userId)
    {
        var dto = new AddWishlistDTO { ProductId = 1, UserId = userId! };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }
}
