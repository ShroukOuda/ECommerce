using ECommerce.Application.DTO.Address;

namespace ECommerce.Application.Validators.Address;

public class UpdateAddressDtoValidator : AddressBaseValidator<UpdateAddressDTO>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
