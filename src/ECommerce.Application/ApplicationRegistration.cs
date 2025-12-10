using System.Reflection;
using ECommerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class ApplicationRegistration 
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
      

        return services;
    }
}