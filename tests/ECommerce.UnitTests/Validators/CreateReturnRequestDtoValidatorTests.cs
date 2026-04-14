using ECommerce.Application.DTO.Return;
using ECommerce.Application.Validators.Return;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreateReturnRequestDtoValidatorTests
{
    private readonly CreateReturnRequestDtoValidator _validator = new();

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            UserId = "user1",
            Reason = "Defective product",
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithOrderIdZero_ShouldFail()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = Guid.Empty,
            UserId = "user1",
            Reason = "Defective",
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyUserId_ShouldFail(string? userId)
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            UserId = userId!,
            Reason = "Defective",
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WithEmptyReason_ShouldFail(string? reason)
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            UserId = "user1",
            Reason = reason!,
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Reason");
    }

    [Fact]
    public async Task Validate_WithReasonTooLong_ShouldFail()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            UserId = "user1",
            Reason = new string('A', 1001),
            Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), ProductId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Reason");
    }

    [Fact]
    public async Task Validate_WithEmptyItems_ShouldFail()
    {
        var dto = new CreateReturnRequestDTO
        {
            OrderId = TestGuid.FromInt(1),
            UserId = "user1",
            Reason = "Defective",
            Items = new List<CreateReturnItemDTO>()
        };
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }
}
