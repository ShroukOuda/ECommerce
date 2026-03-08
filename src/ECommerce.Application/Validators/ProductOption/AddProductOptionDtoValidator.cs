using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Validators.ProductOption;

public class AddProductOptionDtoValidator : AbstractValidator<AddProductOptionDTO>
{
    public AddProductOptionDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DisplayType).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.AttributeKey).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}
