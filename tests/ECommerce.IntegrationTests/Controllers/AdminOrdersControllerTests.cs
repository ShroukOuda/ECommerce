using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Payments;
using ECommerce.Domain.Enums.Order;
using ECommerce.Domain.Enums.Payment;
using ECommerce.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ECommerce.IntegrationTests.Controllers;

public class AdminOrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AdminOrdersControllerTests(CustomWebApplicationFactory factory)
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
            (HttpMethod.Get, "/api/v1/admin/orders"),
            (HttpMethod.Get, "/api/v1/admin/orders/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Put, "/api/v1/admin/orders/00000000-0000-0000-0000-000000000000/status"),
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
            (HttpMethod.Get, "/api/v1/admin/orders"),
            (HttpMethod.Get, "/api/v1/admin/orders/00000000-0000-0000-0000-000000000000"),
            (HttpMethod.Put, "/api/v1/admin/orders/00000000-0000-0000-0000-000000000000/status"),
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
        var response = await _client.GetAsync("/api/v1/admin/orders");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Processing)]
    [InlineData(OrderStatus.OnHold)]
    public async Task AdminCancellationOfPaidOrder_Returns409AndPersistsNoChanges(OrderStatus status)
    {
        using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();
        var orderId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        // Factory uses a unique InMemory store, no external DB or schema creation.
        using (var scope = factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Set<Order>().Add(new() { Id = orderId, UserId = "test-user", OrderStatus = status });
            context.Set<Payment>().Add(new() { Id = paymentId, OrderId = orderId, UserId = "test-user", Status = PaymentStatus.Paid });
            await context.SaveChangesAsync();
        }
        for (var attempt = 0; attempt < 2; attempt++)
        {
            var response = await client.PutAsJsonAsync($"/api/v1/admin/orders/{orderId}/status", new { orderStatus = "Cancelled" });
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            (await response.Content.ReadAsStringAsync()).Should().Contain("manual review");
        }
        using var check = factory.Services.CreateScope();
        var reloaded = check.ServiceProvider.GetRequiredService<AppDbContext>();
        (await reloaded.Set<Order>().SingleAsync(o => o.Id == orderId)).OrderStatus.Should().Be(status);
        (await reloaded.Set<Payment>().SingleAsync(p => p.Id == paymentId)).Status.Should().Be(PaymentStatus.Paid);
        (await reloaded.Set<OrderStatusHistory>().CountAsync(h => h.OrderId == orderId)).Should().Be(0);
    }
}
