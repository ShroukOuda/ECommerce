using ECommerce.Application.DTO.Address;

namespace ECommerce.Application.Validators.Address;

public class AddAddressDtoValidator : AddressBaseValidator<AddAddressDTO>
{
    public AddAddressDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}
