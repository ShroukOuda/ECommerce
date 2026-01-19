namespace ECommerce.Application.Validators.Category;

public class UpdateCategoryDtoValidator : CategoryBaseValidator<UpdateCategoryDTO>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Category Id must be greater than zero.");
    }
}