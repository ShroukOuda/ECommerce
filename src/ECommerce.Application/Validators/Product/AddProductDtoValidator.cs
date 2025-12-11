using FluentValidation;

namespace ECommerce.Application.Validators.Product;

public class AddProductDtoValidator : ProductBaseValidator<AddProductDTO>
{
    public AddProductDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than zero.");
        
    }
}