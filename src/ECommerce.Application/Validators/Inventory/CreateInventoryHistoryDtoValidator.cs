using ECommerce.Application.DTO.Inventory;

namespace ECommerce.Application.Validators.Inventory;

public class CreateInventoryHistoryDtoValidator : AbstractValidator<CreateInventoryHistoryDTO>
{
    public CreateInventoryHistoryDtoValidator()
    {
        RuleFor(x => x.ChangeType).NotEmpty().IsInEnum();
        RuleFor(x => x.ProductVariantId).NotEmpty().WithMessage("Choose a product variant.");
        RuleFor(x => x.QuantityChange).NotEqual(0);
    }
}
