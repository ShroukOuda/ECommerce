# ========================================
# Stage 1: Build
# ========================================

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# ========================================
# Copy project files
# ========================================

COPY src/ECommerce.Domain/ECommerce.Domain.csproj \
     src/ECommerce.Domain/

COPY src/ECommerce.Application/ECommerce.Application.csproj \
     src/ECommerce.Application/

COPY src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj \
     src/ECommerce.Infrastructure/

COPY src/ECommerce.API/ECommerce.API.csproj \
     src/ECommerce.API/

# ========================================
# Restore API dependencies
# ========================================

RUN dotnet restore src/ECommerce.API/ECommerce.API.csproj \
    --no-cache \
    --force

# ========================================
# Copy source code
# ========================================

COPY src/ src/

# ========================================
# Publish API
# ========================================

RUN dotnet publish src/ECommerce.API/ECommerce.API.csproj \
    -c Release \
    -o /app/publish


# ========================================
# Stage 2: Runtime
# ========================================

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Install curl for health check
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

# Create required directories
RUN mkdir -p /app/wwwroot/images /app/logs

# Copy published application
COPY --from=build /app/publish .

# ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

# Health check
HEALTHCHECK \
    --interval=30s \
    --timeout=10s \
    --start-period=60s \
    --retries=5 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start API
ENTRYPOINT ["dotnet", "ECommerce.API.dll"]