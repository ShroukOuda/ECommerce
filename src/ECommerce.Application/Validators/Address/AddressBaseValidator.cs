using ECommerce.Application.DTO.Address;

namespace ECommerce.Application.Validators.Address;

public class AddressBaseValidator<T> : AbstractValidator<T> where T : AddressBaseDTO
{
    public AddressBaseValidator()
    {
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AddressLine2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).NotEmpty();
    }
}
