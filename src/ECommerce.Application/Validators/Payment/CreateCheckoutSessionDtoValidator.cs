using ECommerce.Application.DTO.Payment;

namespace ECommerce.Application.Validators.Payment;

public class CreateCheckoutSessionDtoValidator : AbstractValidator<CreateCheckoutSessionDTO>
{
    public CreateCheckoutSessionDtoValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("OrderId is required.");
    }
}