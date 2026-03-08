namespace ECommerce.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    // Category
    public ICategoryRepository CategoryRepository { get; }
    public ICategoryImageRepository CategoryImageRepository { get; }
    
    // Product
    public IProductRepository ProductRepository { get; }
    public IProductImageRepository ProductImageRepository { get; }
    public IProductOptionRepository ProductOptionRepository { get; }
    public IProductOptionValueRepository ProductOptionValueRepository { get; }
    public IProductVariantRepository ProductVariantRepository { get; }
    public IProductVariantOptionValueRepository ProductVariantOptionValueRepository { get; }
    
    // Brand
    public IBrandRepository BrandRepository { get; }
    public IBrandLogoRepository BrandLogoRepository { get; }
    
    // Order
    public IOrderRepository OrderRepository { get; }
    public IOrderItemRepository OrderItemRepository { get; }
    public IOrderItemOptionRepository OrderItemOptionRepository { get; }
    public IOrderStatusHistoryRepository OrderStatusHistoryRepository { get; }
    
    // Cart
    public ICartRepository CartRepository { get; }
    public ICartItemRepository CartItemRepository { get; }
    public ICartItemOptionRepository CartItemOptionRepository { get; }
    
    // Coupon
    public ICouponRepository CouponRepository { get; }
    public ICouponUsageRepository CouponUsageRepository { get; }
    
    // Review
    public IProductReviewRepository ProductReviewRepository { get; }
    public IReviewHelpfulVoteRepository ReviewHelpfulVoteRepository { get; }
    
    // Wishlist
    public IWishlistRepository WishlistRepository { get; }
    
    // Payment
    public IPaymentRepository PaymentRepository { get; }
    
    // Shipping
    public IShippingRepository ShippingRepository { get; }
    
    // Return
    public IReturnRequestRepository ReturnRequestRepository { get; }
    public IReturnItemRepository ReturnItemRepository { get; }
    
    // Inventory
    public IInventoryHistoryRepository InventoryHistoryRepository { get; }
    
    // User
    public IAddressRepository AddressRepository { get; }
    public IUserSessionRepository UserSessionRepository { get; }
    
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        
        CategoryRepository = new CategoryRepository(_context);
        CategoryImageRepository = new CategoryImageRepository(_context);
        
        ProductRepository = new ProductRepository(_context);
        ProductImageRepository = new ProductImageRepository(_context);
        ProductOptionRepository = new ProductOptionRepository(_context);
        ProductOptionValueRepository = new ProductOptionValueRepository(_context);
        ProductVariantRepository = new ProductVariantRepository(_context);
        ProductVariantOptionValueRepository = new ProductVariantOptionValueRepository(_context);
        
        BrandRepository = new BrandRepository(_context);
        BrandLogoRepository = new BrandLogoRepository(_context);
        
        OrderRepository = new OrderRepository(_context);
        OrderItemRepository = new OrderItemRepository(_context);
        OrderItemOptionRepository = new OrderItemOptionRepository(_context);
        OrderStatusHistoryRepository = new OrderStatusHistoryRepository(_context);
        
        CartRepository = new CartRepository(_context);
        CartItemRepository = new CartItemRepository(_context);
        CartItemOptionRepository = new CartItemOptionRepository(_context);
        
        CouponRepository = new CouponRepository(_context);
        CouponUsageRepository = new CouponUsageRepository(_context);
        
        ProductReviewRepository = new ProductReviewRepository(_context);
        ReviewHelpfulVoteRepository = new ReviewHelpfulVoteRepository(_context);
        
        WishlistRepository = new WishlistRepository(_context);
        
        PaymentRepository = new PaymentRepository(_context);
        
        ShippingRepository = new ShippingRepository(_context);
        
        ReturnRequestRepository = new ReturnRequestRepository(_context);
        ReturnItemRepository = new ReturnItemRepository(_context);
        
        InventoryHistoryRepository = new InventoryHistoryRepository(_context);
        
        AddressRepository = new AddressRepository(_context);
        UserSessionRepository = new UserSessionRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}