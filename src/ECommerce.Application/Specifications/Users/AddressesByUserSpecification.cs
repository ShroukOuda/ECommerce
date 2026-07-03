using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Users;

namespace ECommerce.Application.Specifications.Users;

public class AddressesByUserSpecification : BaseSpecification<Address, Guid>
{
    public AddressesByUserSpecification(string userId) 
        : base(a => a.UserId == userId)
    {
        AsNoTracking();
    }

    
}