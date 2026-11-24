# Development guide

## Safe start

Use .NET 9 SDK. Restore with `dotnet restore` and build with `dotnet build ECommerce.sln`.

The owner generates and reviews migrations locally, uploads them herself and manages database updates. On 24 November she clarified that this does not prohibit startup application of existing migrations and approved restoring the original behavior: startup calls Database.Migrate() when AUTO_MIGRATE=true OR the environment is Development. Development therefore auto-applies pending migrations even when AUTO_MIGRATE=false. Startup does not scaffold migration files. No application startup, migration execution, EnsureCreated fallback or database mutation was performed here. Existing migration files and snapshot remain untouched. Review the CategorySubscription model/snapshot mismatch before starting against SQL: EF Core may reject pending model changes until the owner provides a reviewed migration.

Put local connection strings, JWT signing material, SMTP and Stripe test keys in untracked development configuration or environment variables. The checked-in appsettings file is a sample, not working credentials. Never commit private configuration.

After the owner prepares an isolated database, run `dotnet run --project src/ECommerce.API`. Use the listening URL printed by ASP.NET, not a hardcoded documentation port. Swagger is available only in Development. `/api/v1/health` is liveness only, not database readiness.

## Seeding

`DataInitialization:Enabled` defaults to false. It calls runtime `IDataSeeder`, not HasData, when explicitly enabled. It can create users and business records. Do not enable it against development or production data as part of this remediation. Tests explicitly enable it only after replacing SQL Server with a unique in-memory store.

## Architecture

Domain contains entities/enums/repository contracts; Application contains DTOs, validators, services, mappings and specifications; Infrastructure implements persistence/providers; API owns authenticated HTTP boundaries. Registered async DTO validators run before controller actions, and services retain defensive business checks.

For tests see [TESTING.md](TESTING.md). For unresolved release gates see [PHASE0-REMEDIATION.md](PHASE0-REMEDIATION.md). For test payments see [PAYMENT-GUIDE.md](PAYMENT-GUIDE.md).

## Owner-only CategorySubscription migration review

The only entity/configuration schema delta in this branch is the CategorySubscription inverse navigation correction. In the checked-in snapshot/initial migration, CategorySubscriptions has CategoryId and nullable shadow CategoryId1, including IX_CategorySubscriptions_CategoryId1 and FK_CategorySubscriptions_Categories_CategoryId1. The corrected mapping uses CategoryId for Category.CategorySubscriptions. Do not assume CategoryId1 is empty or blindly drop it. Review actual migration history, both columns and their associations, back up data, then inspect the scaffolded Up/Down before applying or uploading it. A generated migration may contain unrelated drift; do not accept that without review.

Suggested migration name: FixCategorySubscriptionRelationship. From the repository root, only the owner runs the following on her device after that review:

```bash
dotnet ef migrations add FixCategorySubscriptionRelationship --project src/ECommerce.Infrastructure --startup-project src/ECommerce.API
```

After reviewing the generated migration and selecting the intended private connection/environment, the owner may update her intended database:

```bash
dotnet ef database update --project src/ECommerce.Infrastructure --startup-project src/ECommerce.API
```

These commands are documentation only and were not executed here. Do not use production/dev business data for the still-unapproved parallel SQL tests. Startup auto-apply does not repair a missing migration for a model/snapshot mismatch.
