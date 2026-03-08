using ECommerce.Core.Entities.Cart;

namespace ECommerce.Infrastructure.Repositories;

public class CartItemOptionRepository : GenericRepository<CartItemOption, int>, ICartItemOptionRepository
{
    public CartItemOptionRepository(AppDbContext context) : base(context) { }
}
