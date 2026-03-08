# Docker Setup

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) 24.0+
- [Docker Compose](https://docs.docker.com/compose/install/) v2+
- At least 4 GB RAM allocated to Docker (SQL Server requirement)

## Quick Start

```bash
# Clone the repository
git clone https://github.com/ShroukOuda/ECommerce.git
cd ECommerce

# Start all services
docker compose -f docker/docker-compose.yml up --build

# API will be available at:
# http://localhost:8080/swagger
```

## Services

| Service | Image | Port | Description |
|---------|-------|------|-------------|
| `ecommerce-api` | Custom (Dockerfile) | 8080 | ASP.NET Core 9 Web API |
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | SQL Server 2022 Express |

## Environment Variables

### API Service

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Development` | Runtime environment |
| `ASPNETCORE_URLS` | `http://+:8080` | Listening URL |
| `ConnectionStrings__DefaultConnection` | *(see compose)* | SQL Server connection string |
| `AUTO_MIGRATE` | `true` | Auto-run EF migrations on startup |

### SQL Server

| Variable | Default | Description |
|----------|---------|-------------|
| `SA_PASSWORD` | `Docker123#` | SA password (change in production!) |
| `ACCEPT_EULA` | `Y` | Accept SQL Server EULA |
| `MSSQL_PID` | `Express` | SQL Server edition |

## Volumes

| Volume | Mount Point | Description |
|--------|-------------|-------------|
| `sqlserver-data` | `/var/opt/mssql` | SQL Server data persistence |
| `api-images` | `/app/wwwroot/images` | Uploaded product images |
| `api-logs` | `/app/logs` | Application logs |

## Networking

All services communicate on the `ecommerce-network` bridge network. The API connects to SQL Server using the service name `sqlserver` as the hostname.

## Common Commands

```bash
# Build and start (foreground)
docker compose -f docker/docker-compose.yml up --build

# Start in background
docker compose -f docker/docker-compose.yml up -d --build

# Stop all services
docker compose -f docker/docker-compose.yml down

# Stop and remove volumes (fresh start)
docker compose -f docker/docker-compose.yml down -v

# View logs
docker compose -f docker/docker-compose.yml logs -f ecommerce-api
docker compose -f docker/docker-compose.yml logs -f sqlserver

# Rebuild only the API
docker compose -f docker/docker-compose.yml build ecommerce-api

# Execute command in running container
docker compose -f docker/docker-compose.yml exec ecommerce-api bash

# Check health status
docker compose -f docker/docker-compose.yml ps
```

## Troubleshooting

### SQL Server won't start
- Ensure Docker has at least 4 GB of RAM allocated
- Check logs: `docker compose -f docker/docker-compose.yml logs sqlserver`
- On Apple Silicon Macs, use `--platform linux/amd64` or Azure SQL Edge

### API fails to connect to SQL Server
- SQL Server needs 60-90 seconds to initialize on first run
- The `depends_on` with `service_healthy` ensures the API waits
- Verify the connection string matches the SA password

### Port conflicts
- If port 1433 is in use: change `"1433:1433"` → `"1434:1433"` in docker-compose.yml
- If port 8080 is in use: change `"8080:8080"` → `"5000:8080"` in docker-compose.yml

### Migrations fail
- Check that `AUTO_MIGRATE=true` is set
- Ensure the database is reachable before the app starts (health checks handle this)
- Try removing volumes for a clean reset: `docker compose down -v`

## Production Deployment Notes

For production deployments:

1. **Change passwords**: Update `SA_PASSWORD` and connection string
2. **Use secrets**: Store passwords in Docker secrets or environment-specific config
3. **Set environment**: Change `ASPNETCORE_ENVIRONMENT` to `Production`
4. **Disable auto-migrate**: Set `AUTO_MIGRATE=false` and run migrations manually
5. **Enable HTTPS**: Add a reverse proxy (nginx/Caddy) with TLS termination
6. **Resource limits**: Add `deploy.resources.limits` for CPU/memory
7. **Logging**: Configure structured logging with a log aggregation service
8. **Backups**: Set up automated SQL Server backups from the data volume
