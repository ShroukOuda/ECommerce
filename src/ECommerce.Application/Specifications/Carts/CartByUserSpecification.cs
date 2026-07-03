using ECommerce.Domain.Specifications.Base;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Enums.Cart;

namespace ECommerce.Application.Specifications.Carts;

public class CartByUserSpecification : BaseSpecification<Cart, Guid>
{
    public CartByUserSpecification(string userId)
        : base(c => c.UserId == userId)
    {
        AddInclude(c => c.CartItems);
        AddOrderByDescending(c => c.CreatedAt);
        AsNoTracking();
    }

    public CartByUserSpecification(string userId, CartStatus status)
        : base(c => c.UserId == userId && c.Status == status)
    {
        AddInclude(c => c.CartItems);
        AddOrderByDescending(c => c.CreatedAt);
        AsNoTracking();
    }

    
}