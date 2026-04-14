using ECommerce.Application.DTO.Brand;

namespace ECommerce.Application.Validators.Brand;

public class UpdateBrandDtoValidator : BrandBaseValidator<UpdateBrandDTO>
{
    public UpdateBrandDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Brand ID is required.");
    }
}
