using ECommerce.API.Middleware;
using ECommerce.Domain.Exceptions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ECommerce.IntegrationTests;

public class BusinessErrorMappingTests
{
    [Theory]
    [InlineData("business", 400)]
    [InlineData("missing", 404)]
    [InlineData("unexpected", 500)]
    public async Task ProductionMapsKnownErrorsButHidesUnexpectedMessages(string kind, int expected)
    {
        Exception error = kind switch
        {
            "business" => new BusinessRuleException("Status transition rejected"),
            "missing" => new NotFoundException("Notification not found"),
            _ => new InvalidOperationException("internal diagnostic must stay private")
        };
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var middleware = new ExceptionsMiddleware(_ => throw error, new ProductionEnvironment(), cache, new ConfigurationBuilder().Build());
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        await middleware.InvokeAsync(context);
        context.Response.StatusCode.Should().Be(expected);
        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        if (expected == 500) body.Should().NotContain(error.Message);
        else body.Should().Contain(error.Message);
    }

    private sealed class ProductionEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "ErrorMappingTests";
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
