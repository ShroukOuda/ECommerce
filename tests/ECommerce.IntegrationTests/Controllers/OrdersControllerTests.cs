using System.Net;
using System.Net.Http.Json;
using ECommerce.Domain.Entities.Carts;
using ECommerce.Domain.Entities.Orders;
using ECommerce.Domain.Entities.Products;
using ECommerce.Domain.Enums.Cart;
using ECommerce.Domain.Enums.Product;
using ECommerce.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.IntegrationTests.Controllers;

public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByUserId_WithValidUserId_ShouldReturnSuccessOrEmpty()
    {
        var response = await _client.GetAsync("/api/v1/orders");
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, because: $"Response body: {body}");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnErrorResponse()
    {
        var response = await _client.GetAsync("/api/v1/orders/00000000-0000-0000-0000-000000099999");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateOrderThenCreateReturn_EndToEnd_ShouldRefundPersistedPaidValue()
    {
        var productId = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        var cartId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "E2E Product",
                Status = ProductStatus.Published,
                BasePrice = 100m,
                SalePrice = 0m
            });
            db.ProductVariants.Add(new ProductVariant
            {
                Id = variantId,
                ProductId = productId,
                VariantName = "Standard",
                Status = ProductVariantStatus.Active,
                StockQuantity = 10,
                PriceAdjustment = 0m
            });
            db.Carts.Add(new Cart
            {
                Id = cartId,
                UserId = "user-123",
                Status = CartStatus.Active,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            });
            await db.SaveChangesAsync();
        }


        var orderResponse = await _client.PostAsJsonAsync("/api/v1/orders", new
        {
            cartId,
            currency = "USD",
            items = new[]
            {
                new { productId, productVariantId = variantId, quantity = 2, options = Array.Empty<object>() }
            }
        });

        var orderBody = await orderResponse.Content.ReadAsStringAsync();
        orderResponse.StatusCode.Should().Be(HttpStatusCode.Created, because: $"Response body: {orderBody}");

        Guid orderId;
        Guid orderItemId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var order = await db.Orders
                .Include(o => o.OrderItems)
                .SingleAsync(o => o.UserId == "user-123");

            // Checkout persisted trusted server-side prices.
            order.SubTotal.Should().Be(200m);
            order.TotalAmount.Should().Be(200m);
            var line = order.OrderItems.Should().ContainSingle().Subject;
            line.UnitPrice.Should().Be(100m);
            line.TotalPrice.Should().Be(200m);
            orderId = order.Id;
            orderItemId = line.Id;
        }

        var returnResponse = await _client.PostAsJsonAsync("/api/v1/returns", new
        {
            orderId,
            items = new[]
            {
                new { orderItemId, quantity = 1, reason = "Broken" }
            }
        });

        var returnBody = await returnResponse.Content.ReadAsStringAsync();
        returnResponse.StatusCode.Should().Be(HttpStatusCode.Created, because: $"Response body: {returnBody}");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var returnRequest = await db.ReturnRequests
                .Include(r => r.ReturnItems)
                .SingleAsync(r => r.OrderId == orderId);

            // 1 of 2 units at 100 paid per unit refunds 100.
            returnRequest.RefundAmount.Should().Be(100m);
            returnRequest.ReturnItems.Should().ContainSingle()
                .Which.RefundAmount.Should().Be(100m);
        }
    }
}
