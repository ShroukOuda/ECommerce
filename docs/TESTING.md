# Testing Guide

## Test Projects Overview

| Project | Type | Tests | Description |
|---------|------|-------|-------------|
| `ECommerce.UnitTests` | Unit | 304 | Service logic & validator tests with Moq |
| `ECommerce.IntegrationTests` | Integration | 68 | API endpoint tests with WebApplicationFactory |
| `ECommerce.ArchitectureTests` | Architecture | 9 | Clean architecture enforcement with NetArchTest |

**Total: 381 tests**

## Running Tests

### All Tests

```bash
dotnet test
```

### Unit Tests Only

```bash
dotnet test tests/ECommerce.UnitTests
```

### Integration Tests Only

```bash
dotnet test tests/ECommerce.IntegrationTests
```

### Architecture Tests Only

```bash
dotnet test tests/ECommerce.ArchitectureTests
```

### With Code Coverage

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Verbose Output

```bash
dotnet test --verbosity normal
```

## Test Structure

### Unit Tests (`ECommerce.UnitTests`)

```
Services/
  AddressServiceTests.cs        # CRUD, validation, not-found handling
  BrandServiceTests.cs          # GetAll, GetById, Add, Delete, validation
  CartServiceTests.cs           # Cart lookup, add/update/remove items, clear
  CategoryServiceTests.cs       # GetAll, GetById, not-found handling
  CouponServiceTests.cs         # CRUD, code lookup, validation
  InventoryServiceTests.cs      # History lookup, stock adjustments, guards
  NotificationEventServiceTests.cs
  NotificationPreferenceServiceTests.cs
  OrderServiceTests.cs          # Customer/admin flows, cancel and status rules
  PaymentServiceTests.cs
  ProductOptionServiceTests.cs  # Options and option values
  ProductServiceTests.cs
  ProductVariantServiceTests.cs
  ReturnServiceTests.cs         # Customer requests and admin status machine
  ReviewServiceTests.cs
  ShippingServiceTests.cs
  WishlistServiceTests.cs
Validators/
  Add*/Create*/Update*DtoValidatorTests.cs # Valid/invalid inputs, edge cases
```

**Patterns used:**
- **Arrange-Act-Assert** for all tests
- **Moq** for mocking repositories and IUnitOfWork
- **FluentAssertions** for readable assertions
- Services are tested in isolation from the database

### Integration Tests (`ECommerce.IntegrationTests`)

```
Controllers/
  *ControllerTests.cs          # One class per /api/v1/* controller
CustomWebApplicationFactory.cs # In-memory DB setup, service overrides
TestAuthenticationHandler.cs   # Authenticated test user (Admin + Customer roles)
```

**Patterns used:**
- **WebApplicationFactory** with custom factory
- **InMemory EF Core** database (replaces SQL Server)
- Real HTTP requests through the full middleware pipeline
- **NullFileProvider** replaces PhysicalFileProvider for test environment

### Architecture Tests (`ECommerce.ArchitectureTests`)

```
ArchitectureTests.cs
  - Core has no dependency on Application
  - Core has no dependency on Infrastructure
  - Core has no dependency on API
  - Application has no dependency on Infrastructure
  - Application has no dependency on API
  - Entities don't depend on Application
  - Services end with "Service"
  - Interfaces start with "I"
  - Controllers inherit BaseController
```

## Recent Test-Suite Realignment (fix/unit-tests-and-controllers)

The suite was rewritten to match the refactored order/returns/admin code:

- Validator tests for `Update*DTO` no longer test an `Id` property: update DTOs
  dropped `Id` (the id now comes from the route), and the validators dropped the
  `Id` rules with it. The removed `WithZeroId` tests covered a rule that no longer exists.
- `UserId` tests were removed from `CreateOrderDTO` / `AddReviewDTO` /
  `CreateReturnRequestDTO` validator tests: user identity now comes from the JWT,
  not the request body, so there is no `UserId` field to validate.
- Coupon and shipping tests now use the `DiscountType` / `ShippingMethod` enums
  instead of strings.
- Inventory history tests were rewritten for the redesigned DTO
  (`QuantityChange`, `InventoryChangeType`, optional `ProductVariantId`).
- Integration tests now call the real `/api/v1/*` routes. Tests for endpoints
  removed in the admin split (public coupon GETs, `/api/Homepage/banners`,
  `/deals-today`, `/featured-collections`) were replaced with tests for the
  endpoints that replaced them (coupon `validate`/`apply`, single `/api/v1/homepage`).
- `ReturnServiceTests` and `OrderServiceTests` were rewritten to cover the new
  customer/admin flows, including the return status machine and order cancel rules.

## Writing New Tests

### Unit Test Example

```csharp
public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new ProductService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Arrange
        var products = new List<Product> { new() { Id = 1, Name = "Test" } };
        _unitOfWorkMock.Setup(u => u.GetRepository<Product, Guid>().GetAllAsync())
            .ReturnsAsync(products);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
            .Returns(new List<ProductDto> { new() { Id = 1, Name = "Test" } });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
    }
}
```

### Integration Test Example

```csharp
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn200()
    {
        var response = await _client.GetAsync("/api/Products/get-all");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
```

### Architecture Test Example

```csharp
[Fact]
public void Domain_ShouldNotDependOn_Infrastructure()
{
    var result = Types.InAssembly(typeof(Product).Assembly)
        .ShouldNot()
        .HaveDependencyOn("ECommerce.Infrastructure")
        .GetResult();

    result.IsSuccessful.Should().BeTrue();
}
```

## CI/CD Integration

Add to your GitHub Actions workflow:

```yaml
- name: Run Tests
  run: dotnet test --verbosity normal --logger "trx;LogFileName=results.trx"

- name: Run Tests with Coverage
  run: dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Coverage Reports

Generate HTML coverage reports:

```bash
# Install report generator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage

# Generate HTML report
reportgenerator -reports:coverage/**/coverage.cobertura.xml \
  -targetdir:coverage/report -reporttypes:Html

# Open report
open coverage/report/index.html
```

## Phase 2 Additions (authorization, error paths, return integrity)

**Admin and auth controller coverage (new):** 11 admin controllers (AdminBrands, AdminCategories, AdminCoupons, AdminOrders, AdminProducts, AdminProductOptions, AdminProductVariants, AdminReturnItems, AdminReturns, AdminShipping, NotificationPreferences) and AuthenticationController now have integration tests focused on authorization (401 unauthenticated, 403 customer-only). For each admin controller the authorization tests invoke **every protected action** (all verbs/routes attributed with `[Authorize(Roles = "Admin")]`), chunked to stay under the API rate limit, plus happy-path checks where they are deterministic (admin list GETs, brand/category creation). The test auth handler supports `X-Test-Anonymous: true` (forces 401) and `X-Test-Role: Customer` (strips the Admin role) headers.

**Return creation integrity:** `ReturnService.CreateReturnAsync` now verifies the order exists and belongs to the requesting user (KeyNotFoundException otherwise), that every returned item belongs to the order, and that requested quantities do not exceed purchased minus already-returned quantities (excluding rejected/cancelled returns). Return items are constructed server-side with the ProductId taken from the order item. The validator now also requires a non-empty OrderItemId and Quantity > 0 per item.

**Refund calculation (approved policy):** `ReturnService.CreateReturnAsync` computes each return item's refund as the paid line value (`OrderItem.TotalPrice`) minus its proportional share of the order's coupon discount (allocated by line share of the subtotal, clamped at zero), divided by purchased quantity and multiplied by the returned quantity. Shipping is never refunded, order-level tax is not allocated into item refunds, and no fees are added. `ReturnRequest.RefundAmount` is the sum of its item refunds. Money-bearing hardening: duplicate `OrderItemId` entries in one request are accounted cumulatively so they cannot bypass the per-line quantity limit; the cumulative refunded value per line is capped at the line's net paid value (across prior requests and within the same request), so repeated partial refunds cannot exceed it through per-request rounding. Line totals are gross by contract: the coupon discount only ever exists as a separate order-level `CouponUsage` (cart subtotal math and `CouponUsage.DiscountAmount` confirm it), so allocating it in the refund cannot double-subtract a discount baked into `TotalPrice`.

## Phase 3 Additions (checkout pricing, user-delegated defaults)

**Checkout now persists trusted server-side prices.** `OrderService.CreateOrderAsync` prices every item from the catalog, never from client input: unit price = (`Product.SalePrice` when `IsOnSale`, else `BasePrice`) + `ProductVariant.PriceAdjustment` + option prices resolved from the catalog (`ProductOptionValue.PriceValue` for valued options matched by value/label, else the option's flat `PriceValue`). Line `TotalPrice = UnitPrice * Quantity`; `SubTotal` is the line sum. `ProductName`/`VariantName`/`VariantAttributes` are snapshotted onto the order item. The client-supplied option `PriceAdjustment` is ignored.

**Integrity at checkout:** the cart must exist, belong to the ordering user, be active and unexpired (otherwise 404 / InvalidOperation); products must be published; variants must belong to the product, be active and have sufficient stock; options must belong to the product and valued options must contain the requested value. All violations throw ArgumentException (HTTP 400) before anything is written. The validator now also requires a non-empty `CartId` and per-item `ProductVariantId`.

**Chosen defaults (delegated by the project owner, documented, not legal/tax determinations):**
- `TaxAmount = 0` and `ShippingCost = 0`: no tax or shipping rules exist anywhere in the project; these are explicit zero placeholders until real rules are provided. `TotalAmount = SubTotal + Tax + Shipping`.
- Cart discounts are NOT applied at checkout: `Cart` stores only a discount amount, not which coupon produced it, so applying it would leave no `CouponUsage` trace and refunds could exceed what was paid. Applying coupons at checkout needs a cart-to-coupon link (schema change) and is the one held piece.
- Stock is validated for sufficiency but not decremented: stock-movement timing (placement vs payment vs shipment) and level (both `Product` and `ProductVariant` carry `StockQuantity`; the only existing writer is manual product-level adjustment) is inventory policy that does not exist yet. Cancel does not restock for the same reason.
- Order items still come from the request DTO (existing API contract); the cart supplies checkout context/ownership.

**Two latent bugs fixed (found by the new end-to-end test):**
- `CustomWebApplicationFactory` generated a NEW InMemory database name per context creation (the options lambda evaluated `Guid.NewGuid()` each time), so every scope saw a fresh empty store. The name is now generated once per factory instance.
- `ReturnRequest.RejectionReason` was configured `IsRequired` but the entity left it null on creation, so creating a return through the real pipeline always failed with a 500. The entity now follows the codebase convention (`string.Empty` default).

**End-to-end regression:** `OrdersControllerTests.CreateOrderThenCreateReturn_EndToEnd` seeds a product/variant/cart, creates an order over HTTP (asserting persisted prices and totals), then creates a return over HTTP and asserts the refund equals the pro-rata paid value.

**Removed tautological tests:** `VariantSelectorsControllerTests` asserted only "OK or NotFound" against routes that have no controller; removed as coverage evidence (variant selectors are DTO shapes used inside product-option flows).

**Middleware not-found fix:** `ExceptionsMiddleware` previously mapped all unhandled exceptions (including KeyNotFoundException) to HTTP 500. It now maps KeyNotFoundException to HTTP 404 with an `ApiExceptions` body, matching established API conventions. The Brands get-by-id not-found integration test is the regression test for this fix and now asserts exactly 404.

**Known limitations:** Admin controller actions that require existing entities (update/delete by id, return state transitions) have full authorization coverage but not yet full state-machine integration coverage. The seeded-admin login happy path could not be grounded in the test environment and is not asserted.
