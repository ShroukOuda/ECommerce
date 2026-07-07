# Testing Guide

## Test Projects Overview

| Project | Type | Tests | Description |
|---------|------|-------|-------------|
| `ECommerce.UnitTests` | Unit | 13 | Service logic & validator tests with Moq |
| `ECommerce.IntegrationTests` | Integration | 2 | API endpoint tests with WebApplicationFactory |
| `ECommerce.ArchitectureTests` | Architecture | 9 | Clean architecture enforcement with NetArchTest |

**Total: 24 tests**

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
  BrandServiceTests.cs        # GetAll, GetById, Add, Delete, validation
  CategoryServiceTests.cs     # GetAll, GetById, not-found handling
Validators/
  AddBrandDtoValidatorTests.cs # Valid/invalid inputs, edge cases
```

**Patterns used:**
- **Arrange-Act-Assert** for all tests
- **Moq** for mocking repositories and IUnitOfWork
- **FluentAssertions** for readable assertions
- Services are tested in isolation from the database

### Integration Tests (`ECommerce.IntegrationTests`)

```
Controllers/
  BrandsControllerTests.cs     # GET /api/Brands/get-all, GET by invalid ID
CustomWebApplicationFactory.cs # In-memory DB setup, service overrides
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
