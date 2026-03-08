using ECommerce.Application.DTO.Shipping;

namespace ECommerce.Application.Validators.Shipping;

public class CreateShippingDtoValidator : AbstractValidator<CreateShippingDTO>
{
    public CreateShippingDtoValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.AddressId).GreaterThan(0);
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Method).NotEmpty();
    }
}
