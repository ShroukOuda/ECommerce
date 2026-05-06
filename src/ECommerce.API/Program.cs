using System.Text.Json.Serialization;
using ECommerce.Domain.Configuration;
using ECommerce.API.Middleware;
using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Interfaces.Seed;


namespace ECommerce.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        // Add memory caching
        builder.Services.AddMemoryCache();
        
        // Add HttpContextAccessor for accessing HTTP context in services
        builder.Services.AddHttpContextAccessor();
        
        // Configure controllers and JSON options
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    
        // Configure file validation settings
        builder.Services.Configure<FileValidationSettings>(
            builder.Configuration.GetSection(FileValidationSettings.SectionName));
            
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        //add swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            // Add JWT Authentication to Swagger
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Enter JWT token like: Bearer {your token}"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            
        });
        
        // Add application and infrastructure services
        builder.Services.InfrastructureConfiguratoin(builder.Configuration);
        builder.Services.AddApplicationServices(); 
        
        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontendApps", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:4200", // Angular dev server
                        "http://localhost:3000" // React dev server
                                                
                        )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

           
        
        // Auto-migrate on startup when enabled
        if (builder.Configuration.GetValue<bool>("AUTO_MIGRATE", false) ||
            app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        
            try
            {
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(ex, "Seeding failed — application startup aborted");
                throw;   
            }
        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseCors("AllowFrontendApps");
        
        app.UseMiddleware<ExceptionsMiddleware>();
        
        app.UseHttpsRedirection();
        
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}