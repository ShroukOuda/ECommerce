# ASP.NET Core 9 E-Commerce API 🛒

[![Status](https://img.shields.io/badge/Status-In%20Active%20Development-yellow)](https://github.com/ShroukOuda/ECommerce)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)

A production-ready e-commerce REST API built with ASP.NET Core 9, featuring clean architecture, advanced filtering, input validation, and Docker support.

---

## 📋 Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Usage Examples](#-usage-examples)
- [Configuration](#-configuration)
- [Docker Details](#-docker-details)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)

---

## ✨ Features

### Current Features

- ✅ **Full CRUD Operations** for Products and Categories
- ✅ **Advanced Product Filtering** – Search by name, filter by category, price range
- ✅ **Sorting & Pagination** – Sort by price, name, date; paginated results
- ✅ **Image Management** – Upload, update, and delete product images
- ✅ **Input Validation** – FluentValidation for request validation
- ✅ **Clean Architecture** – Separation into API, Application, Core, Infrastructure layers
- ✅ **Repository + Unit of Work Pattern** – Consistent data access abstraction
- ✅ **DTO Pattern** – Secure data transfer between layers
- ✅ **Global Exception Handling** – Consistent error responses
- ✅ **Swagger/OpenAPI Documentation** – Interactive API testing
- ✅ **Entity Framework Core** – Code-first migrations, SQL Server support
- ✅ **Docker Support** – Containerized deployment with Docker Compose
- ✅ **Seed Data** – Organized seed data for development and testing

### Planned Features

- 🔄 Authentication & Authorization (JWT, Roles)
- 🔄 Shopping Cart & Order Management
- 🔄 User Profiles & Addresses
- 🔄 Payment Integration (Stripe/PayPal)
- 🔄 Email Notifications
- 🔄 Unit & Integration Tests

---

## 🛠️ Technologies

### Core Stack
- **ASP.NET Core 9** – Web API framework
- **Entity Framework Core 9** – ORM for database operations
- **SQL Server 2022** – Primary database
- **C# 12** – Programming language

### Libraries & Tools
- **AutoMapper** – Object-to-object mapping
- **FluentValidation** – Request validation
- **Swashbuckle (Swagger)** – API documentation

### Infrastructure
- **Docker** – Containerization
- **Docker Compose** – Multi-container orchestration

---

## 🏗️ Architecture

### Project Structure

```
ECommerce/
├── src/
│   ├── ECommerce.API/              # 🌐 Presentation Layer
│   │   ├── Controllers/            # API endpoints
│   │   ├── Middleware/             # Custom middleware (exception handling)
│   │   ├── Helper/                 # Response models, pagination
│   │   ├── Program.cs              # Application entry point
│   │   └── appsettings.json        # Configuration
│   │
│   ├── ECommerce.Application/      # 💼 Application Layer
│   │   ├── DTO/
│   │   │   ├── Category/           # Category DTOs
│   │   │   ├── Product/            # Product DTOs
│   │   │   └── Photo/              # Photo DTOs
│   │   ├── Services/               # Business logic implementation
│   │   ├── Interfaces/             # Service contracts
│   │   ├── Validators/             # FluentValidation rules
│   │   └── Mapping/                # AutoMapper profiles
│   │
│   ├── ECommerce.Core/             # 🎯 Domain Layer
│   │   ├── Entities/               # Domain models (Product, Category, Photo)
│   │   ├── Common/                 # Base entities
│   │   ├── Interfaces/
│   │   │   ├── Repositories/       # Repository contracts
│   │   │   └── Services/           # Domain service contracts
│   │   └── Specifications/         # Query specifications, parameters
│   │
│   └── ECommerce.Infrastructure/   # 🔧 Infrastructure Layer
│       ├── Persistence/
│       │   ├── Context/            # DbContext
│       │   ├── Configurations/     # Entity configurations
│       │   ├── Migrations/         # EF Core migrations
│       │   └── Seed/               # Database seed data
│       ├── Repositories/           # Repository implementations
│       ├── Services/               # Infrastructure services
│       └── Settings/               # Configuration models
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
- **Specification Pattern** – Query filtering and sorting

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
   git clone https://github.com/ShroukOuda/ECommerce.git
   cd ECommerce
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
    - Seed initial data
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
   git clone https://github.com/ShroukOuda/ECommerce.git
   cd ECommerce
   dotnet restore
   ```

2. **Configure the database**

   Update `src/ECommerce.API/appsettings.json`:

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
   cd src/ECommerce.API
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
dotnet ef migrations add MigrationName \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API \
  --output-dir Persistence/Migrations

# Remove last migration
dotnet ef migrations remove \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API

# Update database to specific migration
dotnet ef database update MigrationName \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API

# Generate SQL script
dotnet ef migrations script \
  --project src/ECommerce.Infrastructure \
  --startup-project src/ECommerce.API
```

---

## 📚 API Documentation

### Base URL
- **Docker**: `http://localhost:8080`
- **Local HTTPS**: `https://localhost:5001`
- **Local HTTP**: `http://localhost:5000`

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
      "imageUrl": "/images/products/1/laptop-001.jpg",
      "createdAt": "2024-12-10T10:30:00Z",
      "updatedAt": "2024-12-10T10:30:00Z"
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
  "photos": [
    {
      "id": 1,
      "url": "/images/products/1/laptop-001.jpg",
      "isMain": true
    }
  ],
  "createdAt": "2024-12-10T10:30:00Z",
  "updatedAt": "2024-12-10T10:30:00Z"
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
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FileSettings": {
    "MaxFileSizeMB": 5,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif"],
    "ImagePath": "wwwroot/Images/Products"
  }
}
```

#### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

#### Environment Variables (Docker)
```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=Server=sqlserver;Database=ECommerceDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true
```

---

## 🐳 Docker Details

### Docker Compose Configuration

The `docker/docker-compose.yml` file defines two services:

1. **sqlserver** - SQL Server 2022 database
2. **ecommerce-api** - ASP.NET Core API

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

## 🗺️ Roadmap

### Phase 1: Core Features ✅
- [x] Clean Architecture Setup
- [x] Product CRUD Operations
- [x] Category Management
- [x] Filtering, Sorting, Pagination
- [x] Image Upload/Management
- [x] FluentValidation Integration
- [x] Docker Support
- [x] Swagger Documentation
- [x] Organized Seed Data

### Phase 2: Security & User Management 🔄
- [ ] JWT Authentication
- [ ] User Registration & Login
- [ ] Role-Based Authorization (Admin, Customer)
- [ ] Password Reset & Email Verification

### Phase 3: E-Commerce Core 📋
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

### Phase 5: Testing & Quality 📋
- [ ] Unit Tests (xUnit)
- [ ] Integration Tests
- [ ] Architecture Tests
- [ ] API Testing (Postman Collection)

### Phase 6: Performance Enhancements 📋
- [ ] Redis Caching
- [ ] Response Compression
- [ ] Database Query Optimization
- [ ] Image Optimization

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
- Use meaningful variable and method names
- Write XML documentation for public APIs
- Keep methods small and focused
- Update documentation for new features
- Ensure all builds pass

### Reporting Issues

Please use GitHub Issues to report bugs or request features. Include:
- Clear description of the issue
- Steps to reproduce
- Expected vs actual behavior
- Environment details (OS, .NET version, Docker version)
- Screenshots if applicable

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Shrouq Ouda**
- GitHub: [@ShroukOuda](https://github.com/ShroukOuda)

---

## 🙏 Acknowledgments

- ASP.NET Core Team for the excellent framework
- Clean Architecture principles by Robert C. Martin
- The open-source community for amazing libraries

---

**Made with ❤️ using ASP.NET Core 9**