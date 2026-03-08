# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files for layer caching
COPY ECommerce.sln .
COPY src/ECommerce.Core/ECommerce.Core.csproj src/ECommerce.Core/
COPY src/ECommerce.Application/ECommerce.Application.csproj src/ECommerce.Application/
COPY src/ECommerce.Infrastructure/ECommerce.Infrastructure.csproj src/ECommerce.Infrastructure/
COPY src/ECommerce.API/ECommerce.API.csproj src/ECommerce.API/
COPY tests/ECommerce.UnitTests/ECommerce.UnitTests.csproj tests/ECommerce.UnitTests/
COPY tests/ECommerce.IntegrationTests/ECommerce.IntegrationTests.csproj tests/ECommerce.IntegrationTests/
COPY tests/ECommerce.ArchitectureTests/ECommerce.ArchitectureTests.csproj tests/ECommerce.ArchitectureTests/

RUN dotnet restore ECommerce.sln

# Copy everything and publish
COPY . .
RUN dotnet publish src/ECommerce.API/ECommerce.API.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/wwwroot/images

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]