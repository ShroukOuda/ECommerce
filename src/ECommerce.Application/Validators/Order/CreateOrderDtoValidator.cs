using ECommerce.Application.DTO.Order;

namespace ECommerce.Application.Validators.Order;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDTO>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.CartId).NotEmpty();
        // Order items are sourced from the cart server-side at
        // checkout; the Items collection on the DTO is unused.
    }
}
