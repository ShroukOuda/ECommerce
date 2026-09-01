using ECommerce.Application.DTO.ProductOption;

namespace ECommerce.Application.Validators.ProductOption;

public class UpdateProductOptionDtoValidator : AbstractValidator<UpdateProductOptionDTO>
{
    public UpdateProductOptionDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DisplayType).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.AttributeKey).NotEmpty().MaximumLength(100);
    }
}
