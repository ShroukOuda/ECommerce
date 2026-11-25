using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace ECommerce.IntegrationTests;

public class SwaggerOrderRequestTests
{
    [Fact]
    public async Task CreateOrderRequest_SwaggerExample_RequestSchemaHasOnlyClientInputFields()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient(); // starts the host

        var swagger = factory.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");
        var json = swagger.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
        var dumpPath = Environment.GetEnvironmentVariable("SWAGGER_DUMP_PATH");
        if (!string.IsNullOrEmpty(dumpPath))
            await File.WriteAllTextAsync(dumpPath, json);

        using var doc = JsonDocument.Parse(json);
        var schema = doc.RootElement.GetProperty("components").GetProperty("schemas").GetProperty("CreateOrderDTO");
        schema.GetProperty("properties").EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo("cartId", "shippingAddressId", "billingAddressId");
    }
}
