# Development Guide

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2022](https://www.microsoft.com/sql-server) (or Docker)
- [Docker](https://docs.docker.com/get-docker/) (optional, for containerized setup)
- IDE: [JetBrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)

## Local Setup (without Docker)

### 1. Clone & Restore

```bash
git clone https://github.com/ShroukOuda/ECommerce.git
cd ECommerce
dotnet restore
```

### 2. Configure Database

Update the connection string in `src/ECommerce.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ECommerceDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. Apply Migrations

```bash
dotnet ef database update \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API
```

### 4. Run the API

```bash
dotnet run --project src/ECommerce.API
```

The API will be available at `https://localhost:5001/swagger`.

## Local Setup (with Docker)

```bash
docker compose -f docker/docker-compose.yml up --build
```

API available at `http://localhost:8080/swagger`. See [docs/DOCKER.md](DOCKER.md) for full Docker documentation.

## Running Migrations

### Create a new migration

```bash
dotnet ef migrations add <MigrationName> \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API \
  --output-dir Persistence/Migrations
```

### Apply migrations

```bash
dotnet ef database update \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API
```

### Remove last migration (if not applied)

```bash
dotnet ef migrations remove \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API
```

### Generate SQL script

```bash
dotnet ef migrations script \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API \
  -o migration.sql
```

## Seeding Data

Seed data is applied automatically via EF Core's `HasData()` in `OnModelCreating`. The migration includes:

| Entity | Records | Description |
|--------|---------|-------------|
| Roles | 3 | Admin, Staff, Customer |
| Users | 200 | With hashed passwords (`Password@123`) |
| Addresses | 400+ | 1-3 per user |
| Brands | 30 | With logos |
| Categories | 108 | 4-level hierarchy |
| Products | 80 | Electronics catalog |
| Product Images | 240 | 3 per product |
| Product Options & Variants | 500+ | With cartesian combinations |
| Coupons | 20 | 10 active, 10 expired |
| Orders | 500 | With items, options, status history |
| Payments | 500 | One per order |
| Shipping | 375 | For shipped/delivered orders |
| Reviews | 1000+ | With helpful votes |
| Wishlists | 300 | Across 150 users |
| Carts | 50 | Active carts with items |
| Returns | 50 | With return items |
| Inventory History | 295 | Stock tracking records |
| Coupon Usage | 166 | For orders using coupons |

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ConnectionStrings__DefaultConnection` | Database connection string | *(appsettings)* |
| `AUTO_MIGRATE` | Auto-run migrations on startup | `false` |

## Debugging in Rider/VS

### Rider
1. Open `ECommerce.sln`
2. Set `ECommerce.API` as the startup project
3. Press **Shift+F10** to run or **Shift+F9** to debug

### Visual Studio
1. Open `ECommerce.sln`
2. Right-click `ECommerce.API` → Set as Startup Project
3. Press **F5** to debug

## Code Style & Conventions

- **Architecture**: Clean Architecture (Core → Application → Infrastructure → API)
- **Naming**: PascalCase for public members, camelCase for private fields with `_` prefix
- **DTOs**: Separate Add/Update/Get DTOs per entity
- **Validation**: FluentValidation validators in `Application/Validators/`
- **Mapping**: AutoMapper profiles in `Application/Mapping/`
- **Repository Pattern**: Generic repository with specification pattern
- **Unit of Work**: Aggregates all repositories for transactional operations
- **Enums**: Stored as strings via `HasConversion<string>()`

## Adding New Features (Checklist)

1. **Core Layer**
   - [ ] Create entity in `Core/Entities/`
   - [ ] Add enums to `Core/Enums/` if needed
   - [ ] Create repository interface in `Core/Interfaces/Repositories/`
   - [ ] Add entity configuration in `Infrastructure/Persistence/Configuration/`

2. **Infrastructure Layer**
   - [ ] Implement repository in `Infrastructure/Repositories/`
   - [ ] Register repository in `IUnitOfWork`
   - [ ] Add `DbSet` to `AppDbContext`
   - [ ] Create migration

3. **Application Layer**
   - [ ] Create DTOs in `Application/DTO/`
   - [ ] Create mapping profile in `Application/Mapping/`
   - [ ] Create validators in `Application/Validators/`
   - [ ] Create service interface in `Application/Interfaces/`
   - [ ] Implement service in `Application/Services/`
   - [ ] Register service in `ApplicationRegistration.cs`

4. **API Layer**
   - [ ] Create controller inheriting from `BaseController`
   - [ ] Add CRUD endpoints

5. **Tests**
   - [ ] Add unit tests for the service
   - [ ] Add validator tests
   - [ ] Add integration tests for endpoints
   - [ ] Verify architecture tests still pass
