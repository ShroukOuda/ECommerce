# ECommerce API

> An e-commerce marketplace backend built with ASP.NET Core 9 and Clean Architecture. Supports product catalogs with variants and options, shopping carts, orders, coupons, reviews, wishlists, and a stock/subscription-based notification system, with JWT auth, refresh-token rotation, and role-based access for customers and admins. Built with EF Core, SQL Server, AutoMapper, and FluentValidation, with unit, integration, and architecture-enforcement test suites, and a Docker Compose setup for local development

## Review status

Phase 0 remediation is review-ready, not approved for live payments or deployment. See [the resolution matrix](docs/PHASE0-REMEDIATION.md) for remaining SQL concurrency, reconciliation, inventory policy and owner-managed schema gates. The owner generates migrations locally. Per her clarification, startup again applies existing pending migrations when AUTO_MIGRATE=true or the environment is Development. No API/Docker startup or database update was executed during this remediation. Existing migration files and snapshot are unchanged.

## Quick Start

```bash
git clone https://github.com/ShroukOuda/ECommerce.git
cd ECommerce
docker compose -f docker/docker-compose.yml up --build
# Liveness: http://localhost:8080/api/v1/health
# Base compose uses Production, Swagger is disabled. Database preparation is owner-managed.
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
  ECommerce.Domain/          # Entities, enums, interfaces, Exceptions
  ECommerce.Application/     # DTOs, services, mapping, validators, Specifications
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
| Phase 0 remediation | [docs/PHASE0-REMEDIATION.md](docs/PHASE0-REMEDIATION.md) |
| Payment setup | [docs/PAYMENT-GUIDE.md](docs/PAYMENT-GUIDE.md) |
| Payments API | [docs/api/PAYMENTS.md](docs/api/PAYMENTS.md) |
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

The prior README claimed MIT, but no license file is present. License terms await owner confirmation; this remediation does not add legal terms.
