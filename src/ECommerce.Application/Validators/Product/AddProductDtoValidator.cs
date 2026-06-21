using FluentValidation;

namespace ECommerce.Application.Validators.Product;

public class AddProductDtoValidator : ProductBaseValidator<AddProductDTO>
{
    public AddProductDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");
        RuleFor(x => x.BrandId)
            .NotEmpty().WithMessage("BrandId is required.");
        
    }
}