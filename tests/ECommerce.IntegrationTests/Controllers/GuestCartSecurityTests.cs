using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ECommerce.Application.DTO.Cart;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.IntegrationTests.Controllers;

public class GuestCartSecurityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    public GuestCartSecurityTests(CustomWebApplicationFactory factory) => _factory = factory;

    private async Task<(HttpClient client, string id, string token)> GuestSession()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Anonymous", "true");
        var response = await client.PostAsync("/api/v1/cart/session", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;
        var data = json["data"]!;
        return (client, data["cartId"]!.ToString(), data["guestToken"]!.ToString());
    }

    [Fact]
    public async Task GuestCapability_IsHashedAndRequiredThenRevokedOnClaim()
    {
        var (client, id, token) = await GuestSession();
        token.Length.Should().Be(64);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var stored = await db.Carts.AsNoTracking().SingleAsync(c => c.Id == Guid.Parse(id));
        stored.GuestToken.Should().NotBe(token);
        stored.GuestToken.Length.Should().Be(64);

        (await client.GetAsync($"/api/v1/cart?cartId={id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        client.DefaultRequestHeaders.Add("X-Cart-Token", new string('0', 64));
        (await client.GetAsync($"/api/v1/cart?cartId={id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        client.DefaultRequestHeaders.Remove("X-Cart-Token");
        client.DefaultRequestHeaders.Add("X-Cart-Token", token);
        var allowed = await client.GetAsync($"/api/v1/cart?cartId={id}");
        allowed.StatusCode.Should().Be(HttpStatusCode.OK);
        (await allowed.Content.ReadAsStringAsync()).Should().NotContain(token);
        (await client.PostAsync($"/api/v1/cart/{id}/claim", null)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await client.PostAsJsonAsync("/api/v1/orders", new { cartId = id, currency = "USD" })).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // Add an isolated test item to prove claiming keeps contents and does not silently merge another cart.
        db.CartItems.Add(new CartItem { Id = Guid.NewGuid(), CartId = Guid.Parse(id), Quantity = 2, Price = 12.34m });
        await db.SaveChangesAsync();
        var signedIn = _factory.CreateClient();
        signedIn.DefaultRequestHeaders.Add("X-Cart-Token", token);
        var claimed = await signedIn.PostAsync($"/api/v1/cart/{id}/claim", null);
        claimed.StatusCode.Should().Be(HttpStatusCode.OK);
        var claimedJson = JsonNode.Parse(await claimed.Content.ReadAsStringAsync())!;
        claimedJson["data"]!["items"]!.AsArray().Should().ContainSingle();
        (await client.GetAsync($"/api/v1/cart?cartId={id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
        (await signedIn.GetAsync($"/api/v1/cart?cartId={id}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await signedIn.PostAsync($"/api/v1/cart/{id}/claim", null)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AnonymousCaller_CannotMutateOwnedCartEvenWithItsId()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/v1/cart/session", null);
        var data = JsonNode.Parse(await response.Content.ReadAsStringAsync())!["data"]!;
        var id = data["cartId"]!.ToString();
        client.DefaultRequestHeaders.Add("X-Test-Anonymous", "true");
        (await client.DeleteAsync($"/api/v1/cart/{id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ExpiredGuestCapability_CannotReadOrClaim()
    {
        var (client, id, token) = await GuestSession();
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var cart = await db.Carts.FindAsync(Guid.Parse(id));
        cart!.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);
        await db.SaveChangesAsync();
        client.DefaultRequestHeaders.Add("X-Cart-Token", token);
        (await client.GetAsync($"/api/v1/cart?cartId={id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
