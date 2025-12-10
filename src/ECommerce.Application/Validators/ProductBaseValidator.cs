

namespace ECommerce.Application.Validators;

public class ProductBaseValidator<T> : AbstractValidator<T> where T : ProductBaseDTO
{
    public ProductBaseValidator()
    {
        RuleFor(x => x).NotNull().WithMessage("Product data must be provided.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");


        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
            
        
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.");
        
    }
}