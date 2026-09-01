using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Validators.ProductOption;

public class AddProductOptionValueDtoValidator : AbstractValidator<AddProductOptionValueDTO>
{
    public AddProductOptionValueDtoValidator()
    {
        RuleFor(x => x.Value).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Label).NotEmpty().MaximumLength(200);
    }
}
