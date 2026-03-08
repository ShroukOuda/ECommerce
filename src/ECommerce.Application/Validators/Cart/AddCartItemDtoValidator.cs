using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Validators.Cart;

public class AddCartItemDtoValidator : AbstractValidator<AddCartItemDTO>
{
    public AddCartItemDtoValidator()
    {
        RuleFor(x => x.CartId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
