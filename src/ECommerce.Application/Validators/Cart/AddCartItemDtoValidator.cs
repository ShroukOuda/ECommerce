using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Validators.Cart;

public class AddCartItemDtoValidator : AbstractValidator<AddCartItemDTO>
{
    public AddCartItemDtoValidator()
    {
        RuleFor(x => x.CartId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
