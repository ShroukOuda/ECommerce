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
        var response = await _client.GetAsync("/api/Homepage");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetFeaturedCollections_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Homepage/featured-collections");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetBanners_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Homepage/banners");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetDealsToday_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Homepage/deals-today");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }
}
