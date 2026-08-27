using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class NotificationsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public NotificationsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPreferences_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Notifications/preferences/user-123");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetUnreadCount_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Notifications/unread-count/user-123");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }
}
