using ECommerce.Application.DTO.Wishlist;

namespace ECommerce.Application.Validators.Wishlist;

public class AddWishlistDtoValidator : AbstractValidator<AddWishlistDTO>
{
    public AddWishlistDtoValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.UserId).NotEmpty();
    }
}
