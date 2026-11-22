using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class AdminReturnsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AdminReturnsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedActions_WithoutAuthentication_Batch1_ShouldReturn401()
    {
        // Dedicated factory: the API rate-limits per host (8 req / 30s).
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var actions = new (HttpMethod Method, string Url)[]
        {
            (HttpMethod.Get, "/api/v1/admin/returns"),
            (HttpMethod.Get, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/approve"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/reject"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/in-transit"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/received"),
        };

        var failures = new List<string>();
        foreach (var (method, url) in actions)
        {
            var request = client.CreateRequest(method, url, anonymous: true);
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                failures.Add($"{method} {url} returned {(int)response.StatusCode}");
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task ProtectedActions_WithCustomerRoleOnly_Batch1_ShouldReturn403()
    {
        // Dedicated factory: the API rate-limits per host (8 req / 30s).
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var actions = new (HttpMethod Method, string Url)[]
        {
            (HttpMethod.Get, "/api/v1/admin/returns"),
            (HttpMethod.Get, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/approve"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/reject"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/in-transit"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/received"),
        };

        var failures = new List<string>();
        foreach (var (method, url) in actions)
        {
            var request = client.CreateRequest(method, url, customerOnly: true);
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.Forbidden)
                failures.Add($"{method} {url} returned {(int)response.StatusCode}");
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task ProtectedActions_WithoutAuthentication_Batch2_ShouldReturn401()
    {
        // Dedicated factory: the API rate-limits per host (8 req / 30s).
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var actions = new (HttpMethod Method, string Url)[]
        {
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/processing"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/refund"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/complete"),
        };

        var failures = new List<string>();
        foreach (var (method, url) in actions)
        {
            var request = client.CreateRequest(method, url, anonymous: true);
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.Unauthorized)
                failures.Add($"{method} {url} returned {(int)response.StatusCode}");
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task ProtectedActions_WithCustomerRoleOnly_Batch2_ShouldReturn403()
    {
        // Dedicated factory: the API rate-limits per host (8 req / 30s).
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var actions = new (HttpMethod Method, string Url)[]
        {
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/processing"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/refund"),
            (HttpMethod.Put, "/api/v1/admin/returns/00000000-0000-0000-0000-000000000000/complete"),
        };

        var failures = new List<string>();
        foreach (var (method, url) in actions)
        {
            var request = client.CreateRequest(method, url, customerOnly: true);
            var response = await client.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.Forbidden)
                failures.Add($"{method} {url} returned {(int)response.StatusCode}");
        }

        failures.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_AsAdmin_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/v1/admin/returns");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
