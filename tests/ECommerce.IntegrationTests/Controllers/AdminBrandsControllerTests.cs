using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class AdminBrandsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AdminBrandsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedActions_WithoutAuthentication_ShouldReturn401()
    {
        // Dedicated factory: the API rate-limits per host (8 req / 30s).
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var actions = new (HttpMethod Method, string Url)[]
        {
            (HttpMethod.Post, "/api/v1/admin/brands"),
            (HttpMethod.Put, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Delete, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Post, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000/logos"),
            (HttpMethod.Delete, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000/logos/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Delete, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000/logos"),
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
    public async Task ProtectedActions_WithCustomerRoleOnly_ShouldReturn403()
    {
        // Dedicated factory: the API rate-limits per host (8 req / 30s).
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();
        var actions = new (HttpMethod Method, string Url)[]
        {
            (HttpMethod.Post, "/api/v1/admin/brands"),
            (HttpMethod.Put, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Delete, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Post, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000/logos"),
            (HttpMethod.Delete, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000/logos/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Delete, "/api/v1/admin/brands/00000000-0000-0000-0000-000000000000/logos"),
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
    public async Task Add_AsAdmin_WithValidData_ShouldReturn201()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/admin/brands", new { name = "Test Brand" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
