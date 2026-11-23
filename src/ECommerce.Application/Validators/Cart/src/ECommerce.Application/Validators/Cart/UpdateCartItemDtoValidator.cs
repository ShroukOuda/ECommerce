using ECommerce.Application.DTO.Cart;
namespace ECommerce.Application.Validators.Cart;
public class UpdateCartItemDtoValidator : AbstractValidator<UpdateCartItemDTO>
{
    public UpdateCartItemDtoValidator() => RuleFor(x => x.Quantity).GreaterThan(0);
}
