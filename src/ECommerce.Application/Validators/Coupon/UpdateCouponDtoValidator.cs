using ECommerce.Application.DTO.Coupon;

namespace ECommerce.Application.Validators.Coupon;

public class UpdateCouponDtoValidator : CouponBaseValidator<UpdateCouponDTO>
{
    public UpdateCouponDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
