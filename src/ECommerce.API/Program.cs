using System.Text.Json.Serialization;
using ECommerce.Domain.Configuration;
using ECommerce.API.Middleware;
using ECommerce.Application;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Persistence.Context;
using ECommerce.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        
        builder.Services.AddMemoryCache();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        
       

        builder.Services.Configure<FileValidationSettings>(
            builder.Configuration.GetSection(FileValidationSettings.SectionName));

        builder.Services.Configure<FileStorageSettings>(
            builder.Configuration.GetSection(FileStorageSettings.SectionName));
        
        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection(JwtOptions.SectionName));
        
      
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        //add swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
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