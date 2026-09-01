using ECommerce.Application.DTO.Inventory;

namespace ECommerce.Application.Validators.Inventory;

public class CreateInventoryHistoryDtoValidator : AbstractValidator<CreateInventoryHistoryDTO>
{
    public CreateInventoryHistoryDtoValidator()
    {
        RuleFor(x => x.ChangeType).NotEmpty();
    }
}
