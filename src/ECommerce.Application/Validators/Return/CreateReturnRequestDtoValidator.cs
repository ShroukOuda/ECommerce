using ECommerce.Application.DTO.Return;

namespace ECommerce.Application.Validators.Return;

public class CreateReturnRequestDtoValidator : AbstractValidator<CreateReturnRequestDTO>
{
    public CreateReturnRequestDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one return item is required.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.OrderItemId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
