# Docker setup

Base compose uses Production and does not run migrations or seed data. It is not a production deployment recipe. SQL credentials in compose are local examples and must be replaced privately before any shared deployment.

After owner-managed schema preparation, the owner may use:

```bash
docker compose -f docker/docker-compose.yml up --build
```

API port is 8080. Liveness is `/api/v1/health`, it does not test SQL connectivity. Swagger is disabled in base Production mode.

For Development, explicitly add the override:

```bash
docker compose -f docker/docker-compose.yml -f docker/docker-compose.override.yml up --build
```

The override selects the SDK development stage, uses the API project source directory and corrected source mounts. Swagger is available in Development. This remediation did not start Docker or create any SQL database/volume.

`DataInitialization__Enabled=false` stays false. `AUTO_MIGRATE` is removed and ignored. Stripe pricing is fixed server USD; the legacy Stripe currency setting cannot override persisted/catalog currency. Supply private `Stripe__SecretKey`, `Stripe__WebhookSecret` and frontend `Stripe__ClientUrl` only for an approved isolated test run. See [PAYMENT-GUIDE.md](PAYMENT-GUIDE.md).

Do not delete volumes to fix schema mismatches. Preserve data and ask the owner to review her database history and pending model change. Existing migrations and snapshot are deliberately unchanged.

TLS termination, secure secrets, backups, monitoring and SQL concurrency release gates remain requirements before deployment. The Dockerfile/Compose corrections were reviewed statically, not validated by running containers.
