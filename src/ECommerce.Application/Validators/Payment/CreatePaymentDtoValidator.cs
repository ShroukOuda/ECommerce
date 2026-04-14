using ECommerce.Application.DTO.Payment;

namespace ECommerce.Application.Validators.Payment;

public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDTO>
{
    public CreatePaymentDtoValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Method).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
    }
}
