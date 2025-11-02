using System.Reflection;
using AutoMapper;
using E_Commerece.Application.Interfaces;
using E_Commerece.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerece.Application;

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