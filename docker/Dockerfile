FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["E-Commerece.sln", "."]
COPY ["E-Commerece.Api/E-Commerece.Api.csproj", "E-Commerece.Api/"]
COPY ["E-Commerece.Application/E-Commerece.Application.csproj", "E-Commerece.Application/"]
COPY ["E-Commerece.Core/E-Commerece.Core.csproj", "E-Commerece.Core/"]
COPY ["E-Commerece.Infrastructure/E-Commerece.Infrastructure.csproj", "E-Commerece.Infrastructure/"]
RUN dotnet restore "E-Commerece.sln"

COPY . .
WORKDIR "/src/E-Commerece.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Commerece.Api.dll"]
