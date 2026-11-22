using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class AuthenticationControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthenticationControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Logout_WithoutAuthentication_ShouldReturn401()
    {
        var request = _client.CreateRequest(
            HttpMethod.Post, "/api/v1/auth/logout", anonymous: true,
            content: JsonContent.Create("some-refresh-token"));
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task LogoutAll_WithoutAuthentication_ShouldReturn401()
    {
        var request = _client.CreateRequest(
            HttpMethod.Post, "/api/v1/auth/logout-all", anonymous: true);
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ShouldReturnError()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "nobody@nowhere.test",
            password = "Whatever@123"
        });
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturnError()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@ecommerce.dev",
            password = "WrongPassword@1"
        });
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_ShouldReturnError()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/auth/refresh", "not-a-real-token");
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError);
    }
}
