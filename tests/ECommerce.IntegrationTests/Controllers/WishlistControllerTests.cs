using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class WishlistControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WishlistControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByUser_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Wishlist/get-by-user/test-user-id");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }
}
