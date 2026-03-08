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
    public async Task GetHistory_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Inventory/history/1");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }
}
