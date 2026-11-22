using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class InventoryControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InventoryControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHistory_WithUnknownProduct_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/v1/admin/inventory/00000000-0000-0000-0000-000000099999/history");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
