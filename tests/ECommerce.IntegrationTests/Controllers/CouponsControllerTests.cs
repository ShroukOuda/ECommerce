using System.Net;
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
    public async Task GetAll_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Coupons/get-all");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/Coupons/get-by-id/99999");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetByCode_WithInvalidCode_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/Coupons/get-by-code/INVALIDCODE");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError, HttpStatusCode.NoContent);
    }
}
