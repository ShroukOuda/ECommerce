using ECommerce.Application.DTO.Coupon;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Coupons;
using ECommerce.Domain.Enums.Coupon;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Application.Specifications.Coupons;
using ECommerce.Application.Specifications.Carts;

namespace ECommerce.Application.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICartService _cartService;

    private readonly IValidator<AddCouponDTO> _addValidator;
    private readonly IValidator<UpdateCouponDTO> _updateValidator;

    public CouponService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICartService cartService,
        IValidator<AddCouponDTO> addValidator,
        IValidator<UpdateCouponDTO> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cartService = cartService;
        _addValidator = addValidator;
        _updateValidator = updateValidator;
    }


    public async Task<IEnumerable<GetCouponDTO>> GetAllCouponsAsync()
    {
        var coupons =
            await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .GetAllAsync();

        return _mapper.Map<IEnumerable<GetCouponDTO>>(coupons);
    }

    public async Task<GetCouponDetailsDTO> GetCouponByIdAsync(Guid id)
    {
        var coupon =
            await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .GetByIdAsync(id);

        if (coupon is null)
            throw new KeyNotFoundException(
                $"Coupon with ID {id} not found.");

        return _mapper.Map<GetCouponDetailsDTO>(coupon);
    }

    public async Task<GetCouponDTO?> GetCouponByCodeAsync(string code)
    {
        var spec = new CouponByCodeSpecification(code);

        var coupon =
            await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(spec);

        return coupon is null
            ? null
            : _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task<GetCouponDTO> AddCouponAsync(AddCouponDTO dto)
    {
        var result = await _addValidator.ValidateAsync(dto);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var coupon = _mapper.Map<Coupon>(dto);

        await _unitOfWork
            .GetRepository<Coupon, Guid>()
            .AddAsync(coupon);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task<GetCouponDTO> UpdateCouponAsync(
        Guid id,
        UpdateCouponDTO dto)
    {
        var result = await _updateValidator.ValidateAsync(dto);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var spec = new CouponSpecification(id);

        bool exists =
            await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .ExistsAsync(spec);

        if (!exists)
            throw new KeyNotFoundException(
                $"Coupon with ID {id} not found.");

        var coupon = _mapper.Map<Coupon>(dto);
        coupon.Id = id;

        _unitOfWork
            .GetRepository<Coupon, Guid>()
            .Update(coupon);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<GetCouponDTO>(coupon);
    }

    public async Task DeleteCouponAsync(Guid id)
    {
        var spec = new CouponSpecification(id);

        bool exists =
            await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .ExistsAsync(spec);

        if (!exists)
            throw new KeyNotFoundException(
                $"Coupon with ID {id} not found.");

        var stub = new Coupon { Id = id };

        _unitOfWork
            .GetRepository<Coupon, Guid>()
            .Delete(stub);

        await _unitOfWork.SaveChangesAsync();
    }

    // ==========================================
    // VALIDATE COUPON
    // ==========================================

    public async Task<CouponResultDTO> ValidateCouponAsync(
        string userId,
        ValidateCouponDTO dto)
    {
        var coupon = await GetCouponEntityByCodeAsync(dto.Code);

        var cart = await GetCartAsync(dto.CartId);

        await ValidateCouponAsync(
            coupon,
            cart,
            userId);

        var subtotal = CalculateSubtotal(cart);

        var discount = CalculateDiscount(
            coupon,
            subtotal);

        return new CouponResultDTO
        {
            Code = coupon.Code,
            Subtotal = subtotal,
            DiscountAmount = discount,
            TotalAfterDiscount = subtotal - discount
        };
    }

    // ==========================================
    // APPLY COUPON
    // ==========================================

    public async Task<CouponResultDTO> ApplyCouponAsync(
        string userId,
        ApplyCouponDTO dto)
    {
        var coupon = await GetCouponEntityByCodeAsync(dto.Code);

        var cart = await GetCartAsync(dto.CartId);

        await ValidateCouponAsync(
            coupon,
            cart,
            userId);

        var subtotal = CalculateSubtotal(cart);

        var discount = CalculateDiscount(
            coupon,
            subtotal);

        cart.DiscountAmount = discount;

        _unitOfWork
            .GetRepository<Cart, Guid>()
            .Update(cart);

        await _unitOfWork.SaveChangesAsync();

        return new CouponResultDTO
        {
            Code = coupon.Code,
            Subtotal = subtotal,
            DiscountAmount = discount,
            TotalAfterDiscount = subtotal - discount
        };
    }

    // ==========================================
    // GET COUPON
    // ==========================================

    private async Task<Coupon> GetCouponEntityByCodeAsync(
        string code)
    {
        var spec = new CouponByCodeSpecification(code);

        var coupon =
            await _unitOfWork
                .GetRepository<Coupon, Guid>()
                .GetFirstOrDefaultAsync(spec);

        if (coupon is null)
            throw new KeyNotFoundException(
                $"Coupon '{code}' not found.");

        return coupon;
    }

    // ==========================================
    // GET CART
    // ==========================================

    private async Task<Cart> GetCartAsync(Guid cartId)
    {
        var spec = new CartDetailsSpecification(cartId);

        var cart =
            await _unitOfWork
                .GetRepository<Cart, Guid>()
                .GetFirstOrDefaultAsync(spec);

        if (cart is null)
            throw new KeyNotFoundException(
                $"Cart with ID {cartId} not found.");

        return cart;
    }

    // ==========================================
    // VALIDATION
    // ==========================================

    private async Task ValidateCouponAsync(
        Coupon coupon,
        Cart cart,
        string userId)
    {
        if (cart.UserId != userId)
            throw new UnauthorizedAccessException(
                "You cannot use this cart.");

        if (!cart.CartItems.Any())
            throw new InvalidOperationException(
                "Cannot apply a coupon to an empty cart.");

        if (coupon.Status != CouponStatus.Active)
            throw new InvalidOperationException(
                "This coupon is not active.");

        var now = DateTime.UtcNow;

        if (now < coupon.ValidFrom)
            throw new InvalidOperationException(
                "This coupon is not valid yet.");

        if (now > coupon.ValidUntil)
            throw new InvalidOperationException(
                "This coupon has expired.");

        if (coupon.UsageLimit > 0 &&
            coupon.UsedCount >= coupon.UsageLimit)
        {
            throw new InvalidOperationException(
                "This coupon has reached its usage limit.");
        }

        var subtotal = CalculateSubtotal(cart);

        if (subtotal < coupon.MinPurchaseAmount)
        {
            throw new InvalidOperationException(
                $"Minimum purchase amount is {coupon.MinPurchaseAmount}.");
        }

        var spec = new CouponUsageByUserSpecification(coupon.Id, userId);

        var userUsageCount =
            await _unitOfWork
                .GetRepository<CouponUsage, Guid>()
                .CountAsync(spec);

        if (coupon.PerUserLimit > 0 &&
            userUsageCount >= coupon.PerUserLimit)
        {
            throw new InvalidOperationException(
                "You have already reached the usage limit for this coupon.");
        }
    }

    // ==========================================
    // CALCULATE SUBTOTAL
    // ==========================================

    private static decimal CalculateSubtotal(Cart cart)
    {
        return cart.CartItems.Sum(item =>
            item.Price * item.Quantity);
    }

    // ==========================================
    // CALCULATE DISCOUNT
    // ==========================================

    private static decimal CalculateDiscount(
        Coupon coupon,
        decimal subtotal)
    {
        decimal discount;

        if (coupon.DiscountType == DiscountType.Percentage)
        {
            discount =
                subtotal * coupon.DiscountValue / 100m;
        }
        else
        {
            discount = coupon.DiscountValue;
        }

        if (coupon.MaxDiscountAmount > 0)
        {
            discount = Math.Min(
                discount,
                coupon.MaxDiscountAmount);
        }

        return Math.Min(
            discount,
            subtotal);
    }
}