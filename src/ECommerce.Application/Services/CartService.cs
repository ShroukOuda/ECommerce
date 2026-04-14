using ECommerce.Application.DTO.Cart;
using ECommerce.Core.Entities.Cart;
using ECommerce.Core.Interfaces.Repositories;

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
        var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(id, ct);
        if (cart is null) throw new KeyNotFoundException($"Cart with ID {id} not found.");
        return _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var cart = await _unitOfWork.CartRepository.GetActiveCartByUserIdAsync(userId, ct);
        return cart is null ? null : _mapper.Map<GetCartDTO>(cart);
    }

    public async Task AddCartItemAsync(AddCartItemDTO dto, CancellationToken ct = default)
    {
        var result = await _addItemValidator.ValidateAsync(dto, ct);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        var item = _mapper.Map<CartItem>(dto);
        await _unitOfWork.CartItemRepository.AddAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateCartItemAsync(UpdateCartItemDTO dto, CancellationToken ct = default)
    {
        var item = await _unitOfWork.CartItemRepository.GetByIdAsync(dto.Id, ct);
        if (item is null) throw new KeyNotFoundException($"Cart item with ID {dto.Id} not found.");
        item.Quantity = dto.Quantity;
        await _unitOfWork.CartItemRepository.UpdateAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveCartItemAsync(Guid cartItemId, CancellationToken ct = default)
    {
        var item = await _unitOfWork.CartItemRepository.GetByIdAsync(cartItemId, ct);
        if (item is null) throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
        await _unitOfWork.CartItemRepository.DeleteAsync(item, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ClearCartAsync(Guid cartId, CancellationToken ct = default)
    {
        var items = await _unitOfWork.CartItemRepository.GetItemsByCartIdAsync(cartId, ct);
        if (items.Any())
        {
            await _unitOfWork.CartItemRepository.DeleteRangeAsync(items, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
