# ASP.NET Core 9 E-Commerce API 🛒

[![Status](https://img.shields.io/badge/Status-In%20Active%20Development-yellow)](https://github.com/ShroukOuda/E-Commerece)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)



A production-ready e-commerce REST API built with ASP.NET Core 9, featuring clean architecture, advanced filtering, Docker support, and comprehensive product management.

---

## 📋 Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
    - [Prerequisites](#prerequisites)
    - [Docker Deployment](#option-1-docker-deployment-recommended)
    - [Local Development](#option-2-local-development)
- [API Documentation](#-api-documentation)
- [Usage Examples](#-usage-examples)
- [Configuration](#-configuration)
- [Docker Details](#-docker-details)
- [Database](#-database)
- [Testing](#-testing)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)


---

## ✨ Features

### Current Features

- ✅ **Full CRUD Operations** for Products and Categories
- ✅ **Advanced Product Filtering** – Search by name, filter by category, price range
- ✅ **Sorting & Pagination** – Sort by price, name, date; paginated results
- ✅ **Image Management** – Upload, update, and delete product images
- ✅ **Clean Architecture** – Separation into API, Application, Core, Infrastructure layers
- ✅ **Repository + Unit of Work Pattern** – Consistent data access abstraction
- ✅ **DTO Pattern** – Secure data transfer between layers
- ✅ **Global Exception Handling** – Consistent error responses
- ✅ **Swagger/OpenAPI Documentation** – Interactive API testing
- ✅ **Entity Framework Core** – Code-first migrations, SQL Server support
- ✅ **Docker Support** – Containerized deployment with Docker Compose

### Planned Features

- 🔄 Authentication & Authorization (JWT, Roles)
- 🔄 Shopping Cart & Order Management
- 🔄 User Profiles & Addresses
- 🔄 Payment Integration (Stripe/PayPal)
- 🔄 Email Notifications
- 🔄 Redis Caching
- 🔄 Unit & Integration Tests
- 🔄 CI/CD Pipeline
- 🔄 Kubernetes Deployment

---

## 🛠️ Technologies

### Core Stack
- **ASP.NET Core 9** – Web API framework
- **Entity Framework Core 9** – ORM for database operations
- **SQL Server 2022** – Primary database (SQLite for development)
- **C# 12** – Programming language

### Libraries & Tools
- **AutoMapper** – Object-to-object mapping
- **FluentValidation** – Request validation
- **Swashbuckle (Swagger)** – API documentation
- **Serilog** *(planned)* – Structured logging
- **xUnit** *(planned)* – Unit testing framework
- **Moq** *(planned)* – Mocking framework

### Infrastructure
- **Docker** – Containerization
- **Docker Compose** – Multi-container orchestration

---

## 🏗️ Architecture

### Project Structure

```
ECommerceAPI/
├── src/
│   ├── ECommerce.API/              # 🌐 Presentation Layer
│   │   ├── Controllers/            # API endpoints
│   │   ├── Middleware/             # Custom middleware
│   │   ├── Program.cs              # Application entry point
│   │   └── appsettings.json        # Configuration
│   │
│   ├── ECommerce.Application/      # 💼 Application Layer
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Services/               # Business logic
│   │   ├── Validators/             # Input validation
│   │   └── Mappings/               # AutoMapper profiles
│   │
│   ├── ECommerce.Core/             # 🎯 Domain Layer
│   │   ├── Entities/               # Domain models
│   │   ├── Interfaces/             # Repository interfaces
│   │   └── Specifications/         # Query specifications
│   │
│   └── ECommerce.Infrastructure/   # 🔧 Infrastructure Layer
│       ├── Data/                   # DbContext, configurations
│       ├── Repositories/           # Repository implementations
│       └── Migrations/             # EF Core migrations
│
├── tests/                          # 🧪 Test Projects
│   ├── ECommerce.UnitTests/
│   └── ECommerce.IntegrationTests/
│
├── docker/                         # 🐳 Docker Configuration
│   ├── Dockerfile
│   └── docker-compose.yml
│
├── .dockerignore
├── .gitignore
├── ECommerce.sln
└── README.md
```

### Design Patterns

- **Clean Architecture** – Dependency inversion, separation of concerns
- **Repository Pattern** – Data access abstraction
- **Unit of Work Pattern** – Transaction management
- **DTO Pattern** – Data transfer and validation
- **Dependency Injection** – Loose coupling
- **CQRS-lite** – Command/Query separation in services

---

## 🚀 Getting Started

### Prerequisites

#### For Docker Deployment
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (Windows/Mac)
- OR Docker Engine + Docker Compose (Linux)

#### For Local Development
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) or SQL Server LocalDB
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

---

## Option 1: Docker Deployment (Recommended)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/ShroukOuda/E-Commerece.git
   cd E-Commerece
   ```

2. **Start all services**
   ```bash
   docker compose -f docker/docker-compose.yml up -d
   ```

   This command will:
    - Pull SQL Server 2022 image
    - Build the API container
    - Create and configure the database
    - Apply EF Core migrations
    - Start both containers

3. **Verify services are running**
   ```bash
   docker compose -f docker/docker-compose.yml ps
   ```

4. **Access the application**
    - **Swagger UI**: http://localhost:8080/swagger
    - **API Base URL**: http://localhost:8080
    - **SQL Server**: `localhost:1433` (Username: `sa`, Password: `YourStrong!Passw0rd`)

### Docker Management Commands

```bash
# Stop containers
docker compose -f docker/docker-compose.yml down

# Stop and remove volumes (clean slate)
docker compose -f docker/docker-compose.yml down -v

# View logs
docker compose -f docker/docker-compose.yml logs -f

# View API logs only
docker compose -f docker/docker-compose.yml logs -f ecommerce-api

# Restart services
docker compose -f docker/docker-compose.yml restart

# Rebuild and start (after code changes)
docker compose -f docker/docker-compose.yml up -d --build
```

---

## Option 2: Local Development

### Step-by-Step Setup

1. **Clone and restore packages**
   ```bash
   git clone https://github.com/ShroukOuda/E-Commerece.git
   cd ECommerce
   dotnet restore
   ```

2. **Configure the database**

   Update `ECommerce.API/appsettings.json`:

   **For SQL Server LocalDB:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

   **For SQL Server:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ECommerceDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
     }
   }
   ```

   

3. **Apply database migrations**
   ```bash
   cd ECommerce.API
   dotnet ef database update --project ../ECommerce.Infrastructure
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   Or with hot reload:
   ```bash
   dotnet watch run
   ```

5. **Access the application**
    - **HTTPS**: https://localhost:5001/swagger
    - **HTTP**: http://localhost:5000/swagger

### Development Tools

```bash
# Create a new migration
dotnet ef migrations add MigrationName --project src/E-Commerece.Infrastructure --startup-project src/E-Commerece.Api

# Remove last migration
dotnet ef migrations remove --project src/E-Commerece.Infrastructure --startup-project src/E-Commerece.Api

# Update database to specific migration
dotnet ef database update MigrationName --project src/E-Commerece.Infrastructure --startup-project src/E-Commerece.Api

# Generate SQL script
dotnet ef migrations script --project src/E-Commerece.Infrastructure --startup-project src/E-Commerece.Api
```

---

## 📚 API Documentation

### Base URL
- **Docker**: `http://localhost:8080`
- **Local**: `https://localhost:5001` or `http://localhost:5000`

### Swagger UI
Interactive API documentation available at `/swagger`

---

## 🔗 Endpoints

### Categories

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/api/Categories/get-all` | List all categories | `pageNumber`, `pageSize` |
| GET | `/api/Categories/get/{id}` | Get category by ID | `id` (int) |
| POST | `/api/Categories/add` | Create new category | Request body (JSON) |
| PUT | `/api/Categories/update` | Update category | Request body (JSON) |
| DELETE | `/api/Categories/delete/{id}` | Delete category | `id` (int) |

#### Category Request Model
```json
{
  "name": "Electronics",
  "description": "Electronic devices and accessories"
}
```

---

### Products

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | `/api/Products/get-all` | Get products (filtered) | See query parameters below |
| GET | `/api/Products/get/{id}` | Get product by ID | `id` (int) |
| POST | `/api/Products/add` | Create new product | Multipart form data |
| PUT | `/api/Products/update` | Update product | Request body (JSON) |
| DELETE | `/api/Products/delete/{id}` | Delete product | `id` (int) |
| POST | `/api/Products/{id}/upload-image` | Upload/replace image | `id` (int), form file |
| DELETE | `/api/Products/{id}/delete-image` | Delete product image | `id` (int) |

#### Query Parameters for GET /api/Products/get-all

| Parameter | Type | Description | Example |
|-----------|------|-------------|---------|
| `search` | string | Search in product name | `laptop` |
| `categoryId` | int | Filter by category | `1` |
| `minPrice` | decimal | Minimum price | `100.00` |
| `maxPrice` | decimal | Maximum price | `1000.00` |
| `sortBy` | string | Sort order | `price`, `priceDesc`, `name`, `date` |
| `pageNumber` | int | Page number (default: 1) | `1` |
| `pageSize` | int | Items per page (default: 10) | `20` |

#### Product Request Model
```json
{
  "name": "Gaming Laptop",
  "description": "High-performance gaming laptop with RTX 4070",
  "price": 1599.99,
  "categoryId": 1,
  "stockQuantity": 25,
  "sku": "LAPTOP-001"
}
```

---

## 💡 Usage Examples

### Using cURL

#### 1. Get All Products (with filters)
```bash
curl -X GET "http://localhost:8080/api/Products/get-all?search=laptop&categoryId=1&minPrice=500&maxPrice=2000&sortBy=priceDesc&pageNumber=1&pageSize=10"
```

#### 2. Get Single Product
```bash
curl -X GET "http://localhost:8080/api/Products/get/1"
```

#### 3. Create Category
```bash
curl -X POST "http://localhost:8080/api/Categories/add" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Electronics",
    "description": "Electronic devices and gadgets"
  }'
```

#### 4. Create Product (with image)
```bash
curl -X POST "http://localhost:8080/api/Products/add" \
  -F "Name=Gaming Laptop" \
  -F "Description=High performance gaming laptop" \
  -F "Price=1299.99" \
  -F "CategoryId=1" \
  -F "StockQuantity=50" \
  -F "Sku=LAPTOP-001" \
  -F "ImageFile=@/path/to/laptop.jpg"
```

#### 5. Update Product
```bash
curl -X PUT "http://localhost:8080/api/Products/update" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "name": "Updated Gaming Laptop",
    "description": "Updated description",
    "price": 1199.99,
    "categoryId": 1,
    "stockQuantity": 45,
    "sku": "LAPTOP-001"
  }'
```

#### 6. Upload Product Image
```bash
curl -X POST "http://localhost:8080/api/Products/1/upload-image" \
  -F "imageFile=@/path/to/image.jpg"
```

#### 7. Delete Product
```bash
curl -X DELETE "http://localhost:8080/api/Products/delete/1"
```

---

### Using PowerShell

#### Get Products
```powershell
Invoke-RestMethod -Uri "http://localhost:8080/api/Products/get-all?pageSize=5" -Method Get
```

#### Create Category
```powershell
$body = @{
    name = "Electronics"
    description = "Electronic devices"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:8080/api/Categories/add" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

---

### Response Format

#### Success Response (List)
```json
{
  "data": [
    {
      "id": 1,
      "name": "Gaming Laptop",
      "description": "High-performance laptop",
      "price": 1299.99,
      "categoryId": 1,
      "categoryName": "Electronics",
      "stockQuantity": 50,
      "sku": "LAPTOP-001",
      "imageUrl": "/images/products/laptop-001.jpg",
      "createdAt": "2024-01-15T10:30:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalRecords": 48,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### Success Response (Single)
```json
{
  "id": 1,
  "name": "Gaming Laptop",
  "description": "High-performance laptop",
  "price": 1299.99,
  "categoryId": 1,
  "categoryName": "Electronics",
  "stockQuantity": 50,
  "sku": "LAPTOP-001",
  "imageUrl": "/images/products/laptop-001.jpg",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

#### Error Response
```json
{
  "statusCode": 400,
  "message": "Validation failed",
  "errors": [
    "Product name is required",
    "Price must be greater than 0"
  ]
}
```

---

## ⚙️ Configuration

### Application Settings

#### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FileUpload": {
    "MaxFileSizeMB": 5,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif"],
    "ImagePath": "wwwroot/images/products"
  }
}
```

#### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

#### Environment Variables (Docker)
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=sqlserver;Database=ECommerceDb;...
```

---

## 🐳 Docker Details


### Docker Commands

```bash
# Build image manually
docker build -f docker/Dockerfile -t ecommerce-api:latest .

# Run container manually
docker run -d -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e ConnectionStrings__DefaultConnection="Server=sqlserver;..." \
  --name ecommerce-api \
  ecommerce-api:latest

# View container logs
docker logs -f ecommerce-api

# Execute command in container
docker exec -it ecommerce-api bash

# Inspect container
docker inspect ecommerce-api

# Remove container
docker rm -f ecommerce-api
```

---

## 🗄️ Database

### Schema Overview

#### Categories Table
```sql
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2
);
```

#### Products Table
```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    CategoryId INT NOT NULL,
    StockQuantity INT NOT NULL,
    Sku NVARCHAR(50),
    ImageUrl NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
```

### Seed Data

Add sample data in `ECommerce.Infrastructure/Data/DbInitializer.cs`:

```csharp
public static void Seed(ApplicationDbContext context)
{
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category { Name = "Electronics", Description = "Electronic devices" },
            new Category { Name = "Clothing", Description = "Apparel and accessories" },
            new Category { Name = "Books", Description = "Books and magazines" }
        );
        context.SaveChanges();
    }
}
```

---

## 🧪 Testing

### Running Tests (Planned)

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test tests/ECommerce.UnitTests

# Run tests with filter
dotnet test --filter "Category=Unit"
```

### Test Structure (Planned)

```
tests/
├── ECommerce.UnitTests/
│   ├── Services/
│   ├── Validators/
│   └── Repositories/
└── ECommerce.IntegrationTests/
    ├── Controllers/
    └── Database/
```

---

## 🗺️ Roadmap

### Phase 1: Core Features ✅
- [x] Clean Architecture Setup
- [x] Product CRUD Operations
- [x] Category Management
- [x] Filtering, Sorting, Pagination
- [x] Image Upload/Management
- [x] Docker Support
- [x] Swagger Documentation

### Phase 2: Security & User Management 🔄
- [ ] JWT Authentication
- [ ] User Registration & Login
- [ ] Role-Based Authorization (Admin, Customer)
- [ ] Password Reset & Email Verification
- [ ] API Rate Limiting

### Phase 3: E-Commerce Core 🔄
- [ ] Shopping Cart
- [ ] Order Management
- [ ] Order History
- [ ] Inventory Management
- [ ] Product Reviews & Ratings

### Phase 4: Payments & Notifications 📋
- [ ] Stripe Integration
- [ ] PayPal Integration
- [ ] Email Notifications (Order Confirmation, Shipping)
- [ ] Invoice Generation

### Phase 5: Performance & Caching 📋
- [ ] Redis Caching
- [ ] Response Compression
- [ ] CDN Integration for Images
- [ ] Database Query Optimization

### Phase 6: Quality & DevOps 📋
- [ ] Unit Tests (xUnit)
- [ ] Integration Tests
- [ ] Serilog Logging
- [ ] Application Insights
- [ ] CI/CD Pipeline (GitHub Actions)
- [ ] Kubernetes Deployment
- [ ] Health Checks & Monitoring

### Phase 7: Advanced Features 📋
- [ ] Multi-language Support (i18n)
- [ ] Product Recommendations
- [ ] Wishlist
- [ ] Discount Codes & Promotions
- [ ] Analytics Dashboard
- [ ] GraphQL API (Optional)

---

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### How to Contribute

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/AmazingFeature
   ```
3. **Commit your changes**
   ```bash
   git commit -m 'Add some AmazingFeature'
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/AmazingFeature
   ```
5. **Open a Pull Request**

### Code Standards

- Follow C# coding conventions
- Write unit tests for new features
- Update documentation
- Ensure all tests pass
- Keep commits clean and descriptive

### Reporting Issues

Please use GitHub Issues to report bugs or request features. Include:
- Clear description of the issue
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, .NET version, etc.)

---





**Made with ❤️ using ASP.NET Core 9**