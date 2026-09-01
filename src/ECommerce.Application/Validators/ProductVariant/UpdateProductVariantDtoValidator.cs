using ECommerce.Application.DTO.ProductVariant;

namespace ECommerce.Application.Validators.ProductVariant;

public class UpdateProductVariantDtoValidator : AbstractValidator<UpdateProductVariantDTO>
{
    public UpdateProductVariantDtoValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
        RuleFor(x => x.VariantName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Status).NotEmpty();
    }
}
