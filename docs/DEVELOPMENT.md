# Development guide

## Safe start

Use .NET 9 SDK. Restore with `dotnet restore` and build with `dotnet build ECommerce.sln`.

This project follows an owner-managed schema policy. Application startup never creates or migrates a database. Do not use automatic migrations or schema-creation fallbacks. Existing migration files and snapshot must not be edited by this remediation. The owner reviews and generates/applies any future migration herself.

Put local connection strings, JWT signing material, SMTP and Stripe test keys in untracked development configuration or environment variables. The checked-in appsettings file is a sample, not working credentials. Never commit private configuration.

After the owner prepares an isolated database, run `dotnet run --project src/ECommerce.API`. Use the listening URL printed by ASP.NET, not a hardcoded documentation port. Swagger is available only in Development. `/api/v1/health` is liveness only, not database readiness.

## Seeding

`DataInitialization:Enabled` defaults to false. It calls runtime `IDataSeeder`, not HasData, when explicitly enabled. It can create users and business records. Do not enable it against development or production data as part of this remediation. Tests explicitly enable it only after replacing SQL Server with a unique in-memory store.

## Architecture

Domain contains entities/enums/repository contracts; Application contains DTOs, validators, services, mappings and specifications; Infrastructure implements persistence/providers; API owns authenticated HTTP boundaries. Registered async DTO validators run before controller actions, and services retain defensive business checks.

For tests see [TESTING.md](TESTING.md). For unresolved release gates see [PHASE0-REMEDIATION.md](PHASE0-REMEDIATION.md). For test payments see [PAYMENT-GUIDE.md](PAYMENT-GUIDE.md).
