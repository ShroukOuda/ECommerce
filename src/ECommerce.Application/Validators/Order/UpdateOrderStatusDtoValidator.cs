using ECommerce.Application.DTO.Order;

namespace ECommerce.Application.Validators.Order;

public class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDTO>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OrderStatus).NotEmpty().WithMessage("Order status is required.");
    }
}
