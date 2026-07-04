using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Users;

namespace ECommerce.Application.Specifications.Addresses;

public class AddressSpecification : BaseSpecification<Address, Guid>
{
    public AddressSpecification(Guid addressId) 
        : base(a => a.Id == addressId)
    {
        AsNoTracking();
    }

    
}