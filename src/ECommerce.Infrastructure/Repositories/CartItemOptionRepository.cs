using ECommerce.Core.Entities.Cart;

namespace ECommerce.Infrastructure.Repositories;

public class CartItemOptionRepository : GenericRepository<CartItemOption, Guid>, ICartItemOptionRepository
{
    public CartItemOptionRepository(AppDbContext context) : base(context) { }
}
