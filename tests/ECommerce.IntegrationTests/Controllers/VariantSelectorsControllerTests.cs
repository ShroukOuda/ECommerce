using System.Net;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class VariantSelectorsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VariantSelectorsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetVariantSelectors_ShouldReturnSuccessOrNotFound()
    {
        var response = await _client.GetAsync("/api/VariantSelectors/test-product/variant-selectors");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVariants_ShouldReturnSuccessOrNotFound()
    {
        var response = await _client.GetAsync("/api/VariantSelectors/test-product/variants");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }
}
