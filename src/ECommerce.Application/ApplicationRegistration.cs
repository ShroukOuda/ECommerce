using System.Reflection;
using ECommerce.Application.Interfaces.Notifications;
using ECommerce.Application.Services;
using ECommerce.Application.Services.Notifications;
using Microsoft.Extensions.DependencyInjection;


namespace ECommerce.Application;

public static class ApplicationRegistration 
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        // Register application services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddScoped<ICategoryImageService, CategoryImageService>();
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
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationSubscriptionService, NotificationSubscriptionService>();
        services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();
        services.AddScoped<INotificationEventService, NotificationEventService>();

        
        services.AddValidatorsFromAssembly(
            typeof(ApplicationRegistration).Assembly
        );
        
        services.AddAutoMapper(
            cfg => { }, Assembly.GetExecutingAssembly()
        );

        return services;
    }
}