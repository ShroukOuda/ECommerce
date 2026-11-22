using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/v1/products");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/v1/products/00000000-0000-0000-0000-000000099999");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
