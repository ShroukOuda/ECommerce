namespace ECommerce.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    // Category
    ICategoryRepository CategoryRepository { get; }
    ICategoryImageRepository CategoryImageRepository { get; }
    
    // Product
    IProductRepository ProductRepository { get; }
    IProductImageRepository ProductImageRepository { get; }
    IProductOptionRepository ProductOptionRepository { get; }
    IProductOptionValueRepository ProductOptionValueRepository { get; }
    IProductVariantRepository ProductVariantRepository { get; }
    IProductVariantOptionValueRepository ProductVariantOptionValueRepository { get; }
    
    // Brand
    IBrandRepository BrandRepository { get; }
    IBrandLogoRepository BrandLogoRepository { get; }
    
    // Order
    IOrderRepository OrderRepository { get; }
    IOrderItemRepository OrderItemRepository { get; }
    IOrderItemOptionRepository OrderItemOptionRepository { get; }
    IOrderStatusHistoryRepository OrderStatusHistoryRepository { get; }
    
    // Cart
    ICartRepository CartRepository { get; }
    ICartItemRepository CartItemRepository { get; }
    ICartItemOptionRepository CartItemOptionRepository { get; }
    
    // Coupon
    ICouponRepository CouponRepository { get; }
    ICouponUsageRepository CouponUsageRepository { get; }
    
    // Review
    IProductReviewRepository ProductReviewRepository { get; }
    IReviewHelpfulVoteRepository ReviewHelpfulVoteRepository { get; }
    
    // Wishlist
    IWishlistRepository WishlistRepository { get; }
    
    // Payment
    IPaymentRepository PaymentRepository { get; }
    
    // Shipping
    IShippingRepository ShippingRepository { get; }
    
    // Return
    IReturnRequestRepository ReturnRequestRepository { get; }
    IReturnItemRepository ReturnItemRepository { get; }
    
    // Inventory
    IInventoryHistoryRepository InventoryHistoryRepository { get; }
    
    // User
    IAddressRepository AddressRepository { get; }
    IUserSessionRepository UserSessionRepository { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}