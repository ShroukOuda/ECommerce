using ECommerce.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;

namespace ECommerce.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<ECommerce.API.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EmailSettings:Provider"] = "smtp",
                ["EmailSettings:SenderName"] = "MarketNest Test",
                ["EmailSettings:SenderEmail"] = "noreply@test.local",
                ["EmailSettings:SmtpHost"] = "localhost",
                ["EmailSettings:SmtpUser"] = "test-user",
                ["EmailSettings:SmtpPassword"] = "test-password",
                ["EmailSettings:SmtpEnableSsl"] = "false",
                ["EmailSettings:SmtpPort"] = "25",
                ["EmailSettings:SupportEmail"] = "support@test.local",
                ["AdminSeed:UserName"] = "admin",
                ["AdminSeed:FirstName"] = "System",
                ["AdminSeed:LastName"] = "Admin",
                ["AdminSeed:PhoneNumber"] = "+201000000000",
                ["AdminSeed:CountryCode"] = "EG",
                ["AdminSeed:Email"] = "admin@ecommerce.dev",
                ["AdminSeed:Password"] = "Admin@123"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace only the AppDbContext registrations so Identity and auth services remain intact.
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            // Add fresh InMemory database
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid().ToString("N"));
            });

            // Replace IFileProvider
            services.RemoveAll<IFileProvider>();
            services.AddSingleton<IFileProvider>(new NullFileProvider());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.SchemeName,
                _ => { });

            // Ensure the database is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}
