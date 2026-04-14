using ECommerce.Application.DTO.Review;
using ECommerce.Application.Validators.Review;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class AddReviewDtoValidatorTests
{
    private readonly AddReviewDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new AddReviewDTO { Rating = 5, Title = "Great", ProductId = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1), UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public async Task Validate_WithInvalidRating_ShouldFail(int rating)
    {
        var dto = new AddReviewDTO { Rating = rating, Title = "Great", ProductId = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1), UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Rating");
    }

    [Fact]
    public async Task Validate_WithRatingInRange_ShouldPass()
    {
        for (int i = 1; i <= 5; i++)
        {
            var dto = new AddReviewDTO { Rating = i, Title = "Good", ProductId = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1), UserId = "user1" };
            var result = await _validator.ValidateAsync(dto);
            result.IsValid.Should().BeTrue();
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyTitle_ShouldFail(string? title)
    {
        var dto = new AddReviewDTO { Rating = 5, Title = title!, ProductId = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1), UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_WithTitleTooLong_ShouldFail()
    {
        var dto = new AddReviewDTO
        {
            Rating = 5, Title = new string('A', 201), ProductId = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1), UserId = "user1"
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task Validate_WithProductIdZero_ShouldFail()
    {
        var dto = new AddReviewDTO { Rating = 5, Title = "Great", ProductId = Guid.Empty, OrderId = TestGuid.FromInt(1), UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public async Task Validate_WithOrderIdZero_ShouldFail()
    {
        var dto = new AddReviewDTO { Rating = 5, Title = "Great", ProductId = TestGuid.FromInt(1), OrderId = Guid.Empty, UserId = "user1" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public async Task Validate_WithEmptyUserId_ShouldFail()
    {
        var dto = new AddReviewDTO { Rating = 5, Title = "Great", ProductId = TestGuid.FromInt(1), OrderId = TestGuid.FromInt(1), UserId = "" };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }
}
