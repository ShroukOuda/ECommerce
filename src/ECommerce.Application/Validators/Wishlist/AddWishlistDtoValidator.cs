using ECommerce.Application.DTO.Wishlist;

namespace ECommerce.Application.Validators.Wishlist;

public class AddWishlistDtoValidator : AbstractValidator<AddWishlistDTO>
{
    public AddWishlistDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
