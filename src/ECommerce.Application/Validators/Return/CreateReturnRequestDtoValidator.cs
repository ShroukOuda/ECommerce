using ECommerce.Application.DTO.Return;

namespace ECommerce.Application.Validators.Return;

public class CreateReturnRequestDtoValidator : AbstractValidator<CreateReturnRequestDTO>
{
    public CreateReturnRequestDtoValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one return item is required.");
    }
}
