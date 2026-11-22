# Testing Guide

## Test Projects Overview

| Project | Type | Tests | Description |
|---------|------|-------|-------------|
| `ECommerce.UnitTests` | Unit | 278 | Service logic & validator tests with Moq |
| `ECommerce.IntegrationTests` | Integration | 33 | API endpoint tests with WebApplicationFactory |
| `ECommerce.ArchitectureTests` | Architecture | 9 | Clean architecture enforcement with NetArchTest |

**Total: 320 tests**

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
