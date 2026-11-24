using ECommerce.Application.DTO.Inventory;

namespace ECommerce.Application.Validators.Inventory;

public class CreateInventoryHistoryDtoValidator : AbstractValidator<CreateInventoryHistoryDTO>
{
    public CreateInventoryHistoryDtoValidator()
    {
        RuleFor(x => x.ChangeType).NotEmpty().IsInEnum();
        RuleFor(x => x.ProductVariantId).Must(id => id is null || id != Guid.Empty);
        RuleFor(x => x.QuantityChange).NotEqual(0);
    }
}
