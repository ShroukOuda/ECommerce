using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Carts;

namespace ECommerce.Application.Specifications.Carts;

public class CartItemSpecification : BaseSpecification<CartItem, Guid>
{
    public CartItemSpecification(Guid cartItemId)
        : base(ci => ci.Id == cartItemId)
    {
        AsNoTracking();
    }

    
}