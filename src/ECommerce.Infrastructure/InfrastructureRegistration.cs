using System.Text;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities.User;
using ECommerce.Infrastructure.Repositories;
using Ecommerce.Infrastructure.Services;
using ECommerce.Infrastructure.Services;
using ECommerce.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using ECommerce.Infrastructure.Persistence.Seed;
using ECommerce.Application.Interfaces.Seed;
using ECommerce.Infrastructure.Persistence.Seed.Identity;
using ECommerce.Infrastructure.Services.Email;
using ECommerce.Application.Interfaces.Email;

namespace ECommerce.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection InfrastructureConfiguratoin(this IServiceCollection services, IConfiguration configuration)
    {
        //repositories
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
        

        //Settings

        //jwt settings
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        //File storage settings
        services.Configure<FileStorageSettings>(configuration.GetSection(FileStorageSettings.SectionName));

        //admin seed settings
        services.Configure<AdminSeedSettings>(configuration.GetSection(AdminSeedSettings.SectionName));

        //email settings
        services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));

        //app settings
        services.Configure<AppSettings>(configuration.GetSection(AppSettings.SectionName));

        //email template settings
        services.Configure<EmailTemplateSettings>(configuration.GetSection(EmailTemplateSettings.SectionName));

        
        //image management service
        services.AddScoped<IFileStorageService ,FileStorageService>();
        
        //token service
        services.AddScoped<ITokenService, TokenService>();
        
        //phone number service
        services.AddScoped<IPhoneNumberService, PhoneNumberService>();
        
        //request context service
        services.AddScoped<IRequestContextService, RequestContextService>();
        
        //identity service
        services.AddScoped<IIdentityService, IdentityService>();

        //email service
        var emailSettings = configuration.GetSection(EmailSettings.SectionName).Get<EmailSettings>();

        if (emailSettings is null || string.IsNullOrEmpty(emailSettings.Provider))
            throw new Exception("Email configuration is missing or invalid.");

        switch (emailSettings.Provider.ToLower())
        {
            case "smtp":
                services.AddScoped<IEmailService, SmtpEmailService>();
                break;
            case "sendgrid":
                services.AddScoped<IEmailService, SendGridEmailService>();
                break;
            default:
                throw new Exception($"Unsupported email provider: {emailSettings.Provider}");
        }

        services.AddScoped<INotificationEmailService, NotificationEmailService>();
        services.AddScoped<EmailTemplateBuilder>();
        
        
        //token encoder
        services.AddScoped<ITokenEncoder, TokenEncoder>();

        //url builder
        services.AddScoped<IUrlBuilder, UrlBuilder>();

        //data seeder
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<AdminSeeder>();
        services.AddScoped<RoleSeeder>();

        

       
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
        
        // JWT Authentication
        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
        
        if (jwtOptions is null || string.IsNullOrEmpty(jwtOptions.SecretKey))
            throw new Exception("JWT configuration is missing or invalid.");
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });
     
        return services;
    }
}