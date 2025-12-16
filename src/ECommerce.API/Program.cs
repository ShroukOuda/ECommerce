using System.Text.Json.Serialization;
using ECommerce.Core.Configuration;
using ECommerce.API.Middleware;
using ECommerce.Application;
using ECommerce.Application.Validators.Photo;
using ECommerce.Application.Validators.Product;
using ECommerce.Infrastructure;
using ECommerce.Infrastructure.Settings;
using FluentValidation;

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
        
        //Product Validator
        builder.Services.AddValidatorsFromAssembly(typeof(AddProductDtoValidator).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(UpdateProductDtoValidator).Assembly);
        
        //Photo Validator
        builder.Services.AddValidatorsFromAssembly(typeof(UploadPhotoDtoValidator).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(UploadPhotosDtoValidator).Assembly);
        
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

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}