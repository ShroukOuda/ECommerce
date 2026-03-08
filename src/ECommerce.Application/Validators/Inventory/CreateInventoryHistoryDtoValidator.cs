using ECommerce.Application.DTO.Inventory;

namespace ECommerce.Application.Validators.Inventory;

public class CreateInventoryHistoryDtoValidator : AbstractValidator<CreateInventoryHistoryDTO>
{
    public CreateInventoryHistoryDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.NewQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ChangeType).NotEmpty();
    }
}
