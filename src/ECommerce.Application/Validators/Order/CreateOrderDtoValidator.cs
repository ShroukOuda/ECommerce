using ECommerce.Application.DTO.Order;

namespace ECommerce.Application.Validators.Order;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDTO>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Items).NotEmpty().WithMessage("Order must have at least one item.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
