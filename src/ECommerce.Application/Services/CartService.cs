using ECommerce.Application.DTO.Cart;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Application.Specifications.Products;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Product;

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

    public async Task<GetCartDTO> GetCartByIdAsync(Guid id)
    {
        var spec = new CartDetailsSpecification(id);
        var cart = await _unitOfWork.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(spec);
        if (cart is null) throw new KeyNotFoundException($"Cart with ID {id} not found.");
        return _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId)
    {
        var activeCartSpec = new CartByUserSpecification(userId, CartStatus.Active);
        var cart = await _unitOfWork.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(activeCartSpec);
        return cart is null ? null : _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartItemDTO> AddCartItemAsync(AddCartItemDTO dto)
    {
        var result = await _addItemValidator.ValidateAsync(dto);
        if (!result.IsValid) throw new ValidationException(result.Errors);

        // Trusted server-side price snapshot: cart subtotals and
        // coupon math build on this, so it must come from the
        // catalog (effective product price + variant adjustment),
        // never from client input.
        var product = await _unitOfWork
            .GetRepository<Product, Guid>()
            .GetByIdAsync(dto.ProductId);

        if (product is null || product.Status != ProductStatus.Published)
            throw new ArgumentException(
                $"Product {dto.ProductId} is not available.");

        var variant = await _unitOfWork
            .GetRepository<ProductVariant, Guid>()
            .GetFirstOrDefaultAsync(
                new ProductVariantSpecification(dto.VariantId));

        if (variant is null ||
            variant.ProductId != product.Id ||
            variant.Status != ProductVariantStatus.Active)
            throw new ArgumentException(
                $"Product variant {dto.VariantId} is not available for product {product.Id}.");

        var item = _mapper.Map<CartItem>(dto);
        item.Price =
            (product.IsOnSale ? product.SalePrice : product.BasePrice)
            + variant.PriceAdjustment;

        await _unitOfWork.GetRepository<CartItem, Guid>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetCartItemDTO>(item);
    }

    public async Task<GetCartItemDTO> UpdateCartItemAsync(Guid id, UpdateCartItemDTO dto)
    {
        var item = await _unitOfWork.GetRepository<CartItem, Guid>().GetByIdAsync(id);
        if (item is null) throw new KeyNotFoundException($"Cart item with ID {id} not found.");
        item.Quantity = dto.Quantity;
        _unitOfWork.GetRepository<CartItem, Guid>().Update(item);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetCartItemDTO>(item);
    }

    public async Task RemoveCartItemAsync(Guid cartId, Guid cartItemId)
    {
        var spec = new CartItemSpecification(cartId, cartItemId);
        bool exist = await _unitOfWork.GetRepository<CartItem, Guid>().ExistsAsync(spec);
        if (!exist) throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
        var stub = new CartItem { Id = cartItemId };
        _unitOfWork.GetRepository<CartItem, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ClearCartAsync(Guid cartId)
    {
        var spec = new CartItemsByCartSpecification(cartId);
        var items = await _unitOfWork.GetRepository<CartItem, Guid>().GetAllAsync(spec);

        if (items.Any())
        {
            _unitOfWork.GetRepository<CartItem, Guid>().DeleteRange(items);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
