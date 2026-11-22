using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class HomepageControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HomepageControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHomepage_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/homepage");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }
}
