using System.Net;
using System.Net.Http.Json;
using ECommerce.Application.DTO.Brand;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ECommerce.IntegrationTests.Controllers;

public class BrandsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BrandsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/Brands/get-all");

        // Assert - log response body for debugging if not OK
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnErrorResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/Brands/get-by-id/99999");

        // Assert
        // The ExceptionsMiddleware will catch the KeyNotFoundException
        // and return a 404 or handle it accordingly
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }
}
