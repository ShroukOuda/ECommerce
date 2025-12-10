using FluentValidation;

namespace ECommerce.Application.Validators;

public class UpdateProductDtoValidator : ProductBaseValidator<UpdateProductDTO>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid product ID is required.");
        
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than zero.");
    }
}