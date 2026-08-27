# MarketNest API

> A multi-vendor marketplace API for customers and sellers, built with ASP.NET Core 9, Clean Architecture, and SQL Server.

## Quick Start

```bash
git clone https://github.com/ShroukOuda/ECommerce.git
cd ECommerce
docker compose -f docker/docker-compose.yml up --build
# API available at http://localhost:8080/swagger
```

## Tech Stack

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![xUnit](https://img.shields.io/badge/xUnit-Tests-5E1F87)](https://xunit.net/)
[![AutoMapper](https://img.shields.io/badge/AutoMapper-Mapping-BE161D)](https://automapper.org/)
[![FluentValidation](https://img.shields.io/badge/FluentValidation-Validation-1D72B8)](https://docs.fluentvalidation.net/)

## Project Structure

```
src/
  ECommerce.Core/            # Entities, enums, interfaces, specifications
  ECommerce.Application/     # DTOs, services, mapping, validators
  ECommerce.Infrastructure/  # EF Core, repositories, migrations, seed data
  ECommerce.API/             # Controllers, middleware, configuration
tests/
  ECommerce.UnitTests/       # Service & validator unit tests
  ECommerce.IntegrationTests/# API endpoint integration tests
  ECommerce.ArchitectureTests/# Clean architecture enforcement tests
docker/                      # Dockerfile & docker-compose
docs/                        # Full documentation
```

## Documentation

| Topic | Link |
|-------|------|
| API Overview | [docs/api/README.md](docs/api/README.md) |
| Authentication | [docs/api/AUTHENTICATION.md](docs/api/AUTHENTICATION.md) |
| Products | [docs/api/PRODUCTS.md](docs/api/PRODUCTS.md) |
| Categories | [docs/api/CATEGORIES.md](docs/api/CATEGORIES.md) |
| Orders | [docs/api/ORDERS.md](docs/api/ORDERS.md) |
| Cart | [docs/api/CART.md](docs/api/CART.md) |
| Users | [docs/api/USERS.md](docs/api/USERS.md) |
| Reviews | [docs/api/REVIEWS.md](docs/api/REVIEWS.md) |
| Brands | [docs/api/BRANDS.md](docs/api/BRANDS.md) |
| Coupons | [docs/api/COUPONS.md](docs/api/COUPONS.md) |
| Wishlist | [docs/api/WISHLIST.md](docs/api/WISHLIST.md) |
| Variant Selectors | [docs/api/VARIANT_SELECTORS.md](docs/api/VARIANT_SELECTORS.md) |
| Notifications | [docs/api/NOTIFICATIONS.md](docs/api/NOTIFICATIONS.md) |
| Admin | [docs/api/ADMIN.md](docs/api/ADMIN.md) |
| Analytics | [docs/api/ANALYTICS.md](docs/api/ANALYTICS.md) |
| Homepage | [docs/api/HOMEPAGE.md](docs/api/HOMEPAGE.md) |
| Docker Setup | [docs/DOCKER.md](docs/DOCKER.md) |
| Testing Guide | [docs/TESTING.md](docs/TESTING.md) |
| Development Guide | [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) |

## Development Setup

See [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) for local setup, migrations, debugging, and conventions.

## License

This project is licensed under the MIT License.
