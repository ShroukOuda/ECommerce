using ECommerce.Application.DTO.Cart;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Domain.Enums.Cart;

namespace ECommerce.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<AddCartItemDTO> _addItemValidator;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AddCartItemDTO> addItemValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _addItemValidator = addItemValidator;
    }

    public async Task<GetCartDTO> GetCartByIdAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new CartDetailsSpecification(id);
        var cart = await _unitOfWork.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(spec);
        if (cart is null) throw new KeyNotFoundException($"Cart with ID {id} not found.");
        return _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var activeCartSpec = new CartByUserSpecification(userId, CartStatus.Active);
        var cart = await _unitOfWork.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(activeCartSpec);
        return cart is null ? null : _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartItemDTO> AddCartItemAsync(AddCartItemDTO dto, CancellationToken ct = default)
    {
        var result = await _addItemValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var item = _mapper.Map<CartItem>(dto);
        await _unitOfWork.GetRepository<CartItem, Guid>().AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetCartItemDTO>(item);
    }

    public async Task<GetCartItemDTO> UpdateCartItemAsync(Guid id, UpdateCartItemDTO dto, CancellationToken ct = default)
    {
        var item = await _unitOfWork.GetRepository<CartItem, Guid>().GetByIdAsync(id, ct);
        if (item is null) throw new KeyNotFoundException($"Cart item with ID {id} not found.");
        item.Quantity = dto.Quantity;
        _unitOfWork.GetRepository<CartItem, Guid>().Update(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return _mapper.Map<GetCartItemDTO>(item);
    }

    public async Task RemoveCartItemAsync(Guid cartId, Guid cartItemId, CancellationToken ct = default)
    {
        var spec = new CartItemSpecification(cartId, cartItemId);
        bool exist = await _unitOfWork.GetRepository<CartItem, Guid>().ExistsAsync(spec);
        if (!exist) throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
        var stub = new CartItem { Id = cartItemId };
        _unitOfWork.GetRepository<CartItem, Guid>().Delete(stub, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ClearCartAsync(Guid cartId, CancellationToken ct = default)
    {
        var spec = new CartItemsByCartSpecification(cartId);
        var items = await _unitOfWork.GetRepository<CartItem, Guid>().GetAllAsync(spec);

        if (items.Any())
        {
            _unitOfWork.GetRepository<CartItem, Guid>().DeleteRange(items, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
