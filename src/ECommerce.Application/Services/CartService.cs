using ECommerce.Application.DTO.Cart;
using System.Security.Cryptography;
using System.Text;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Carts;
using ECommerce.Application.Specifications.Products;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Product;
using ECommerce.Domain.Enums.Coupon;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Application.Specifications.Coupons;

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

    public async Task<GetCartDTO> GetCartByIdAsync(Guid id, string? userId = null, string? guestToken = null)
    {
        var cart = await GetAccessibleCartAsync(id, userId, guestToken);
        return _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartDTO?> GetActiveCartByUserIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new UnauthorizedAccessException("Sign in to access an account cart.");
        var activeCartSpec = new CartByUserSpecification(userId, CartStatus.Active);
        var cart = await _unitOfWork.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(activeCartSpec);
        return cart is null ? null : _mapper.Map<GetCartDTO>(cart);
    }

    public async Task<GetCartItemDTO> AddCartItemAsync(AddCartItemDTO dto, string? userId = null, string? guestToken = null)
    {
        var validationResult = await _addItemValidator.ValidateAsync(dto);
        if (!validationResult.IsValid) throw new ValidationException(validationResult.Errors);

        var cart = await GetAccessibleCartAsync(dto.CartId, userId, guestToken);
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

        ProductVariant? variant = null;
        if (dto.VariantId.HasValue)
        {
            variant = await _unitOfWork.GetRepository<ProductVariant, Guid>()
                .GetFirstOrDefaultAsync(new ProductVariantSpecification(dto.VariantId.Value));
            if (variant is null || variant.ProductId != product.Id || variant.Status != ProductVariantStatus.Active)
                throw new ArgumentException("Choose an active variant belonging to this product.");
        }
        else if (await _unitOfWork.GetRepository<ProductVariant, Guid>()
            .ExistsAsync(new ProductVariantsByProductSpecification(product.Id)))
            throw new BadRequestException("This product has variants. Choose a variant.");

        var item = _mapper.Map<CartItem>(dto);
        item.Price = (product.IsOnSale ? product.SalePrice : product.BasePrice) + (variant?.PriceAdjustment ?? 0m);

        await _unitOfWork.GetRepository<CartItem, Guid>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync();
        await RefreshCartCouponAsync(cart);
        return _mapper.Map<GetCartItemDTO>(item);
    }

    public async Task<GetCartItemDTO> UpdateCartItemAsync(Guid id, UpdateCartItemDTO dto, string? userId = null, string? guestToken = null)
    {
        var item = await _unitOfWork.GetRepository<CartItem, Guid>().GetByIdAsync(id);
        if (item is null) throw new KeyNotFoundException($"Cart item with ID {id} not found.");
        var cart = await GetAccessibleCartAsync(item.CartId, userId, guestToken);
        if (dto.Quantity <= 0) throw new BadRequestException("Quantity must be positive.");
        item.Quantity = dto.Quantity;
        _unitOfWork.GetRepository<CartItem, Guid>().Update(item);
        await _unitOfWork.SaveChangesAsync();
        await RefreshCartCouponAsync(cart);
        return _mapper.Map<GetCartItemDTO>(item);
    }

    public async Task RemoveCartItemAsync(Guid cartId, Guid cartItemId, string? userId = null, string? guestToken = null)
    {
        var cart = await GetAccessibleCartAsync(cartId, userId, guestToken);
        var spec = new CartItemSpecification(cartId, cartItemId);
        bool exist = await _unitOfWork.GetRepository<CartItem, Guid>().ExistsAsync(spec);
        if (!exist) throw new KeyNotFoundException($"Cart item with ID {cartItemId} not found.");
        var stub = new CartItem { Id = cartItemId };
        _unitOfWork.GetRepository<CartItem, Guid>().Delete(stub);
        await _unitOfWork.SaveChangesAsync();
        await RefreshCartCouponAsync(cart);
    }

    public async Task ClearCartAsync(Guid cartId, string? userId = null, string? guestToken = null)
    {
        var cart = await GetAccessibleCartAsync(cartId, userId, guestToken);
        var spec = new CartItemsByCartSpecification(cartId);
        var items = await _unitOfWork.GetRepository<CartItem, Guid>().GetAllAsync(spec);

        if (items.Any())
        {
            _unitOfWork.GetRepository<CartItem, Guid>().DeleteRange(items);
            await _unitOfWork.SaveChangesAsync();
            await RefreshCartCouponAsync(cart);
        }
    }

    // Keeps the stored coupon discount in step with the cart's items so
    // the cart view never shows a discount computed for old quantities.
    private async Task RefreshCartCouponAsync(Cart cart)
    {
        if (string.IsNullOrWhiteSpace(cart.CouponCode) && cart.DiscountAmount == 0m) return;

        var items = await _unitOfWork.GetRepository<CartItem, Guid>()
            .GetAllAsync(new CartItemsByCartSpecification(cart.Id));
        var subtotal = items.Sum(i => i.Price * i.Quantity);

        var discount = 0m;
        if (!string.IsNullOrWhiteSpace(cart.CouponCode) && subtotal > 0m)
        {
            var coupon = await _unitOfWork.GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(new CouponByCodeSpecification(cart.CouponCode));
            if (coupon is not null && coupon.Status == CouponStatus.Active && subtotal >= coupon.MinPurchaseAmount)
                discount = CouponService.CalculateDiscount(coupon, subtotal);
        }

        // A coupon that no longer applies is removed rather than left stale.
        if (discount == 0m) cart.CouponCode = null;
        cart.DiscountAmount = discount;
        _unitOfWork.GetRepository<Cart, Guid>().Update(cart);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Cart> GetAccessibleCartAsync(Guid id, string? userId, string? guestToken)
    {
        var cart = await _unitOfWork.GetRepository<Cart, Guid>().GetFirstOrDefaultAsync(new CartDetailsSpecification(id));
        var owned = cart is not null && !string.IsNullOrWhiteSpace(cart.UserId)
            && !string.IsNullOrWhiteSpace(userId) && cart.UserId == userId;
        var guest = cart is not null && string.IsNullOrEmpty(cart.UserId)
            && guestToken?.Length == 64 && cart.GuestToken?.Length == 64
            && CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(HashToken(guestToken)), Encoding.ASCII.GetBytes(cart.GuestToken));
        if (cart is null || (!owned && !guest) || cart.Status != CartStatus.Active || cart.ExpiresAt <= DateTime.UtcNow)
            throw new KeyNotFoundException("An accessible active cart was not found.");
        return cart;
    }

    private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.ASCII.GetBytes(token)));

    public async Task<CartSessionDTO> CreateCartAsync(string? userId)
    {
        var token = string.IsNullOrWhiteSpace(userId) ? Convert.ToHexString(RandomNumberGenerator.GetBytes(32)) : null;
        var cart = new Cart { Id = Guid.NewGuid(), UserId = string.IsNullOrWhiteSpace(userId) ? null : userId,
            GuestToken = token is null ? "" : HashToken(token), ExpiresAt = DateTime.UtcNow.AddDays(7), Status = CartStatus.Active };
        await _unitOfWork.GetRepository<Cart, Guid>().AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();
        return new CartSessionDTO { CartId = cart.Id, GuestToken = token, ExpiresAt = cart.ExpiresAt };
    }

    public async Task<GetCartDTO> ClaimGuestCartAsync(Guid cartId, string guestToken, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new UnauthorizedAccessException("Sign in before claiming a cart.");
        var cart = await GetAccessibleCartAsync(cartId, null, guestToken);
        cart.UserId = userId;
        cart.GuestToken = "";
        _unitOfWork.GetRepository<Cart, Guid>().Update(cart);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GetCartDTO>(cart);
    }
}
