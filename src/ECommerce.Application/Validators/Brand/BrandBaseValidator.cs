using ECommerce.Application.DTO.Brand;

namespace ECommerce.Application.Validators.Brand;

public class BrandBaseValidator<T> : AbstractValidator<T> where T : BrandBaseDTO
{
    public BrandBaseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Brand name is required.")
            .MaximumLength(100).WithMessage("Brand name cannot exceed 100 characters.");
    }
}
