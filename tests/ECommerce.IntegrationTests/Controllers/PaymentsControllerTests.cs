using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class PaymentsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PaymentsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByOrder_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/payments/order/00000000-0000-0000-0000-000000099999");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/v1/payments/00000000-0000-0000-0000-000000099999");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
