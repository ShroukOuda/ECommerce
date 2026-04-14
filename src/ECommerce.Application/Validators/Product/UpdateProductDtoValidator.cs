using FluentValidation;

namespace ECommerce.Application.Validators.Product;

public class UpdateProductDtoValidator : ProductBaseValidator<UpdateProductDTO>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required.");
        
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");
    }
}