using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Carts;

namespace ECommerce.Application.Specifications.Carts;

public class CartDetailsSpecification : BaseSpecification<Cart, Guid>
{
    public CartDetailsSpecification(Guid cartId)
        : base(c => c.Id == cartId)
    {
        AddInclude(c => c.CartItems);
        AsNoTracking();
    }

    
}