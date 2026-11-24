# Testing guide

Run from the repository root with .NET 9 SDK:

```bash
dotnet test tests/ECommerce.UnitTests
dotnet test tests/ECommerce.IntegrationTests
dotnet test tests/ECommerce.ArchitectureTests
```

Integration uses a unique EF InMemory database per factory, no nested service provider, no schema creation or migrations, and runtime seed data confined to that store. Test rate-limit threshold is raised explicitly in factory configuration so authorization tests are not hidden behind UI throttling. Production default remains eight requests per thirty seconds per IP. Stripe webhook and liveness bypass that shared-IP UI quota.

Latest executed counts and environment are recorded in [PHASE0-REMEDIATION.md](PHASE0-REMEDIATION.md). Payment unit tests include unpaid completion, financial identity mismatches, terminal statuses, duplicate/unknown events, stale session retrieval and distinct versus same-operation retries. Guest cart HTTP tests cover hashed capability, expiry, bad/missing capability, login requirement at checkout, retained line items on claim and token revocation. Address tests reject cross-owner read/update/delete and owner spoofing at create.

These tests do not prove SQL transactions, unique indexes, concurrency, foreign keys, provider webhook parsing or live Stripe API behavior. No real Stripe requests, Docker startup or development/production database writes were performed. A real isolated SQL concurrency suite and sandbox end-to-end Stripe test remain release gates, not successful tests implied by the in-memory suite.

The earlier valid-product-create baseline fixture now mocks the required category/brand checks. Route-ID update fixtures now arrange loaded entities rather than detached mapping. Consumed-cart replay expects 400 rather than the old accidental 500. Assertions were not changed to accept a range of statuses to conceal regressions.
