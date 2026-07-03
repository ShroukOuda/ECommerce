using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Carts;

namespace ECommerce.Application.Specifications.Carts;

public class CartItemsByCartSpecification : BaseSpecification<CartItem, Guid>
{
    public CartItemsByCartSpecification(Guid cartId)
        : base(ci => ci.CartId == cartId)
    {
        AddInclude(ci => ci.Product);
        AddInclude(ci => ci.CartItemOptions);
        AddInclude(ci => ci.ProductVariant);
        AsNoTracking();
    }

    
}