using ECommerce.Application.DTO.Coupon;

namespace ECommerce.Application.Validators.Coupon;

public class CouponBaseValidator<T> : AbstractValidator<T> where T : CouponBaseDTO
{
    public CouponBaseValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.MinPurchaseAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxDiscountAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.UsageLimit).GreaterThan(0);
        RuleFor(x => x.ValidFrom).LessThan(x => x.ValidUntil).WithMessage("ValidFrom must be before ValidUntil.");
    }
}
