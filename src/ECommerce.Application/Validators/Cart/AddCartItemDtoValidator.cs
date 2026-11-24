using ECommerce.Application.DTO.Cart;

namespace ECommerce.Application.Validators.Cart;

public class AddCartItemDtoValidator : AbstractValidator<AddCartItemDTO>
{
    public AddCartItemDtoValidator()
    {
        RuleFor(x => x.CartId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.VariantId).Must(id => id is null || id != Guid.Empty)
            .WithMessage("Use null for a simple product, not an empty variant ID.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}
