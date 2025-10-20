using E_Commerece.Core.Interfaces;
using E_Commerece.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerece.Infrastructure;

public static class InfrastructureRegisteration
{
    public static IServiceCollection InfrastructureConfiguratoin(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        return services;
    }
}