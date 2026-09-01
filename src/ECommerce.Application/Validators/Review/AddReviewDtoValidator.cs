using ECommerce.Application.DTO.Review;

namespace ECommerce.Application.Validators.Review;

public class AddReviewDtoValidator : AbstractValidator<AddReviewDTO>
{
    public AddReviewDtoValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
