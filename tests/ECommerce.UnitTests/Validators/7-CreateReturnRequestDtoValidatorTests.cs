using ECommerce.Application.DTO.Return;
using ECommerce.Application.Validators.Return;
using FluentAssertions;

namespace ECommerce.UnitTests.Validators;

public class CreateReturnRequestDtoValidatorTests
{
    private readonly CreateReturnRequestDtoValidator _validator = new();

    private static CreateReturnRequestDTO CreateValidDto() => new()
    {
        OrderId = TestGuid.FromInt(1),
        Description = "Defective product",
        Items = new List<CreateReturnItemDTO> { new() { OrderItemId = TestGuid.FromInt(1), Quantity = 1, Reason = "Broken" } }
    };

    [Fact]
    public async Task Validate_WithValidData_ShouldPass()
    {
        var result = await _validator.ValidateAsync(CreateValidDto());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithoutDescription_ShouldPass()
    {
        var dto = CreateValidDto();
        dto.Description = null;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithOrderIdZero_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.OrderId = Guid.Empty;
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public async Task Validate_WithEmptyItems_ShouldFail()
    {
        var dto = CreateValidDto();
        dto.Items = new List<CreateReturnItemDTO>();
        var result = await _validator.ValidateAsync(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }
}
