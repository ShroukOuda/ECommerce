using ECommerce.Application.DTO.Brand;

namespace ECommerce.Application.Validators.Brand;

public class UpdateBrandDtoValidator : BrandBaseValidator<UpdateBrandDTO>
{
    public UpdateBrandDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Brand ID must be greater than zero.");
    }
}
