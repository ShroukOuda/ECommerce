using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class ShippingControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ShippingControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByOrder_WithUnknownOrder_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/v1/shipping/order/00000000-0000-0000-0000-000000099999");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/v1/shipping/00000000-0000-0000-0000-000000099999");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
