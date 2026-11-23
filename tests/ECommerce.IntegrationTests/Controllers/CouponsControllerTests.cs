using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class CouponsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CouponsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Validate_WithUnknownCode_ShouldReturnErrorResponse()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/coupons/validate", new
        {
            cartId = Guid.NewGuid(),
            code = "INVALIDCODE"
        });
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Apply_WithUnknownCode_ShouldReturnErrorResponse()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/coupons/apply", new
        {
            cartId = Guid.NewGuid(),
            code = "INVALIDCODE"
        });
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
    }
}
