namespace ECommerce.Application.Validators.Category;

public class UpdateCategoryDtoValidator : CategoryBaseValidator<UpdateCategoryDTO>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category Id is required.");
    }
}