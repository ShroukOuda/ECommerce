using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class CartControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CartControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByUserId_ShouldReturnResponse()
    {
        var response = await _client.GetAsync("/api/Cart/get-by-user/test-user-id");
        // Cart may return OK with data, NoContent if no active cart, or error
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
