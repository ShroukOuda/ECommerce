
using E_Commerece.Core.Interfaces;
using E_Commerece.Core.Services;
using E_Commerece.Infrastructure.Data;
using E_Commerece.Infrastructure.Repositories;
using E_Commerece.Infrastructure.Repositories.Service;
using E_Commerece.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace E_Commerece.Infrastructure;

public static class InfrastructureRegisteration
{
    public static IServiceCollection InfrastructureConfiguratoin(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        
        //unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        //file provider
        services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
        
        //file settings
        services.Configure<FileSettings>(options => configuration.GetSection("FileSettings").Bind(options));
        
        //image management service
        services.AddScoped<IImageManagementService ,ImageManagementService>();
       
        //db context
        services.AddDbContext<AppDbContext>(option =>
        {
            option.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(60);
                }
            );
        });
        return services;
    }
}