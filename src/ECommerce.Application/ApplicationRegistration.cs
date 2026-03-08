using System.Reflection;
using ECommerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace ECommerce.Application;

public static class ApplicationRegistration 
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Existing services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<ICategoryImageService, CategoryImageService>();
        
        // New domain services
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IShippingService, ShippingService>();
        services.AddScoped<IReturnService, ReturnService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IProductOptionService, ProductOptionService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IUserSessionService, UserSessionService>();
        
        services.AddValidatorsFromAssembly(
            typeof(ApplicationRegistration).Assembly
        );
        
        services.AddAutoMapper(
            cfg => { }, Assembly.GetExecutingAssembly()
        );

        return services;
    }
}