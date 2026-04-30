using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities.User;
using ECommerce.Infrastructure.Repositories;
using Ecommerce.Infrastructure.Services;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace ECommerce.Infrastructure;

public static class InfrastructureRegisteration
{
    public static IServiceCollection InfrastructureConfiguratoin(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        
        //unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        //file provider
        var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        if (Directory.Exists(wwwrootPath))
        {
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));
        }
        else
        {
            Directory.CreateDirectory(wwwrootPath);
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));
        }
        
        
        //image management service
        services.AddScoped<IFileStorageService ,FileStorageService>();
        
        //token service
        services.AddScoped<ITokenService, TokenService>();
        
        //phone number service
        services.AddScoped<IPhoneNumberService, PhoneNumberService>();
        
        //request context service
        services.AddScoped<IRequestContextService, RequestContextService>();
       
        //db context
        services.AddDbContext<AppDbContext>(option =>
        {
            option.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            option.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.CommandTimeout(60);
                    sqlOptions.MigrationsAssembly("ECommerce.Infrastructure");
                }
            );
        });
        
        // Identity
        services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
        
        return services;
    }
}