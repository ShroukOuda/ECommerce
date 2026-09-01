using FluentValidation;

namespace ECommerce.Application.Validators.Product;

public class UpdateProductDtoValidator : ProductBaseValidator<UpdateProductDTO>
{
    public UpdateProductDtoValidator()
    {
        
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required.");
    }
}