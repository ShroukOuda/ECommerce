# ASP.NET Core 9 E-Commerce API 🛒

[![Status](https://img.shields.io/badge/Status-In%20Active%20Development-yellow)](https://github.com/ShroukOuda/ECommerce)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![Progress](https://img.shields.io/badge/Completion-35%25-orange)](https://github.com/ShroukOuda/ECommerce)

A production-ready e-commerce REST API built with ASP.NET Core 9, featuring clean architecture, variant selector system, advanced filtering, and comprehensive business logic.

---

## 📋 Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Architecture](#-architecture)
- [Database Schema](#-database-schema)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Configuration](#-configuration)
- [Contributing](#-contributing)

---

## ✨ Features

### ✅ Implemented (v0.5 - 35% Complete)

#### Core Features
- ✅ **Clean Architecture** – Separation into API, Application, Core, Infrastructure layers
- ✅ **Repository + Unit of Work Pattern** – Consistent data access abstraction
- ✅ **Entity Framework Core 9** – Code-first migrations, SQL Server support
- ✅ **AutoMapper** – Object-to-object mapping
- ✅ **FluentValidation** – Request validation
- ✅ **Global Exception Handling** – Consistent error responses
- ✅ **Swagger/OpenAPI Documentation** – Interactive API testing
- ✅ **Docker Support** – Containerized deployment with Docker Compose

#### Product Management
- ✅ **Product CRUD** – Complete product lifecycle management
- ✅ **Category Management** – Hierarchical category structure
- ✅ **Image Management** – Upload, update, delete product/category images
- ✅ **Advanced Filtering** – Search by name, filter by category, price range
- ✅ **Sorting & Pagination** – Sort by price, name, date; paginated results

#### Data Structures
- ✅ **25+ Domain Entities** – Users, Products, Orders, Cart, Reviews, etc.
- ✅ **40+ Enums** – Type-safe status management
- ✅ **Organized Seed Data** – Development and testing data
- ✅ **5 Entity Configurations** – Category, CategoryImage, Product, ProductImage, Order

### 🔄 In Progress (Current Sprint)

- 🔄 **Product Variant System** – Size, color, material selection (entities ready, configs needed)
- 🔄 **Product Options** – Variant selectors + customizations (entities ready, configs needed)
- 🔄 **Shopping Cart** – Guest and authenticated cart (entities ready, configs needed)
- 🔄 **Entity Configurations** – 21 more needed (see roadmap)

### 📋 Planned Features

#### Phase 1: Core E-Commerce 
- [ ] User Authentication & Authorization (JWT)
- [ ] User Profile Management
- [ ] Address Management
- [ ] Complete Variant Selector System
- [ ] Shopping Cart (Guest + User)
- [ ] Order Management
- [ ] Order Tracking

#### Phase 2: Business Features 
- [ ] Product Reviews & Ratings
- [ ] Wishlist
- [ ] Coupon System
- [ ] Brand Management
- [ ] Inventory Tracking
- [ ] Stock Alerts

#### Phase 3: Payments & Notifications 
- [ ] Payment Integration (Stripe/PayPal)
- [ ] Email Notifications
- [ ] Order Confirmation Emails
- [ ] Invoice Generation
- [ ] Return & Refund System

#### Phase 4: Advanced Features 
- [ ] Admin Dashboard Analytics
- [ ] Sales Reports
- [ ] Customer Analytics
- [ ] Product Recommendations
- [ ] Search Optimization

#### Phase 5: Quality & Performance 
- [ ] Unit Tests (xUnit)
- [ ] Integration Tests
- [ ] Redis Caching
- [ ] Response Compression
- [ ] Query Optimization
- [ ] Image Optimization

---

## 🛠️ Technologies

### Core Stack
- **ASP.NET Core 9** – Web API framework
- **Entity Framework Core 9** – ORM for database operations
- **SQL Server 2022** – Primary database
- **C# 12** – Programming language

### Libraries & Tools
- **AutoMapper 13.0** – Object-to-object mapping
- **FluentValidation 11.9** – Request validation
- **Swashbuckle (Swagger) 6.5** – API documentation

### Future Integrations
- **Redis** – Caching layer (Week 21)
- **Stripe/PayPal** – Payment processing (Week 11-12)
- **SendGrid/SMTP** – Email service (Week 12)
- **xUnit** – Testing framework (Week 19-20)
- **Serilog** – Logging (Week 17)

### Infrastructure
- **Docker** – Containerization
- **Docker Compose** – Multi-container orchestration
- **GitHub Actions** – CI/CD (Future)

---

## 🏗️ Architecture

### Project Structure

```
ECommerce/
├── 📁 ECommerce.API/                    # 🌐 Presentation Layer
│   ├── Controllers/                     # API endpoints
│   │   ├── BaseController.cs
│   │   ├── CategoriesController.cs
│   │   └── ProductsController.cs
│   ├── Extensions/                      # ⭐ TODO: Service registration
│   ├── Filters/                         # ⭐ TODO: Action filters
│   ├── Middleware/
│   │   └── ExceptionsMiddleware.cs     # Global error handling
│   ├── Helper/
│   │   ├── ApiExceptions.cs
│   │   ├── Pagination.cs
│   │   └── ResponseAPI.cs               # API response wrapper
│   ├── wwwroot/Images/                  # Static file storage
│   ├── Program.cs                       # Application entry point
│   └── appsettings.json                 # Configuration
│
├── 📁 ECommerce.Application/            # 💼 Application Layer
│   ├── DTO/                             # Data Transfer Objects
│   │   ├── Category/
│   │   ├── CategoryImages/
│   │   ├── Product/
│   │   ├── ProductImages/
│   │   └── Common/
│   ├── Services/                        # Business logic
│   │   ├── CategoryService.cs
│   │   ├── CategoryImageService.cs
│   │   ├── ProductService.cs
│   │   └── ProductImageService.cs
│   ├── Interfaces/                      # Service contracts
│   ├── Validators/                      # FluentValidation rules
│   │   ├── Category/
│   │   ├── CategoryImage/
│   │   ├── Product/
│   │   ├── ProductImage/
│   │   └── Common/
│   ├── Mapping/                         # AutoMapper profiles
│   ├── Configuration/                   # ⭐ TODO: Move from Core
│   ├── Common/                          # ⭐ TODO: Shared behaviors
│   └── Contracts/                       # ⭐ TODO: Response models
│
├── 📁 ECommerce.Core/                   # 🎯 Domain Layer
│   ├── Entities/                        # 25+ Domain models
│   │   ├── Product/
│   │   │   ├── Product.cs
│   │   │   ├── ProductImage.cs
│   │   │   ├── ProductVariant.cs
│   │   │   ├── ProductOption.cs
│   │   │   ├── ProductOptionValue.cs
│   │   │   └── ProductVariantOptionValue.cs
│   │   ├── Category/
│   │   ├── Brand/
│   │   ├── Cart/
│   │   ├── Order/
│   │   ├── User/
│   │   ├── Review/
│   │   ├── Wishlist/
│   │   ├── Coupon/
│   │   ├── Payment/
│   │   ├── Shipping/
│   │   ├── Return/
│   │   └── Inventory/
│   ├── Enums/                           # 40+ Type-safe enums
│   │   ├── Product/
│   │   │   ├── OptionType.cs           # Variant vs Customization
│   │   │   ├── OptionDisplayType.cs
│   │   │   ├── ProductStatus.cs
│   │   │   └── ProductVariantStatus.cs
│   │   ├── Order/
│   │   ├── Payment/
│   │   ├── Shipping/
│   │   └── ... (40+ total)
│   ├── Common/                          # Base entities
│   │   ├── BaseEntity.cs
│   │   └── BaseImage.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   ├── IGenericRepository.cs
│   │   │   ├── IProductRepository.cs
│   │   │   ├── ICategoryRepository.cs
│   │   │   ├── IProductImageRepository.cs
│   │   │   ├── ICategoryImageRepository.cs
│   │   │   └── IUnitOfWork.cs
│   │   └── Services/
│   │       └── IImageManagementService.cs
│   ├── Exceptions/                      # Domain exceptions
│   │   ├── NotFoundException.cs
│   │   ├── BadRequestException.cs
│   │   └── FileOperationException.cs
│   ├── Specifications/                  # Query specifications
│   │   ├── PaginationParams.cs
│   │   ├── ProductParams.cs
│   │   └── ProductSortBy.cs
│   └── Configuration/                  
│       ├── FileValidationSettings.cs
│       ├── ProductImageValidationSettings.cs
│       └── CategoryImageValidationSettings.cs
│
└── 📁 ECommerce.Infrastructure/         # 🔧 Infrastructure Layer
    ├── Persistence/
    │   ├── Context/
    │   │   └── AppDbContext.cs        
    │   ├── Configurations/              
    │   │   ├── Product/                
    │   │   │   ├── ProductConfiguration.cs
    │   │   │   ├── ProductImageConfiguration.cs 
    │   │   │   ├── ProductVariantConfiguration.cs 
    │   │   │   ├── ProductOptionConfiguration.cs 
    │   │   │   ├── ProductOptionValueConfiguration.cs 
    │   │   │   └── ProductVariantOptionValueConfiguration.cs 
    │   │   ├── Category/
    │   │   │   ├── CategoryConfiguration.cs 
    │   │   │   └── CategoryImageConfiguration.cs 
    │   │   ├── Order/
    │   │   │   ├── OrderConfiguration.cs 
    │   │   │   ├── OrderItemConfiguration.cs 
    │   │   │   ├── OrderItemOptionConfiguration.cs 
    │   │   │   └── OrderStatusHistoryConfiguration.cs 
    │   │   ├── Cart/                   
    │   │   ├── User/                   
    │   │   ├── Brand/                   
    │   │   ├── Review/                  
    │   │   ├── Coupon/                 
    │   │   ├── Wishlist/               
    │   │   ├── Payment/                
    │   │   ├── Shipping/               
    │   │   ├── Return/                  
    │   │   └── Inventory/               
    │   ├── Migrations/
    │   │   ├── 20260101190823_Init.cs
    │   │   ├── 20260101190823_Init.Designer.cs
    │   │   └── AppDbContextModelSnapshot.cs
    │   └── Seed/                        # Database seed data
    │       ├── CategorySeed.cs
    │       ├── CategoryImageSeed.cs
    │       ├── ProductSeed.cs
    │       ├── ProductImageSeed.cs
    │       └── ModelBuilderExtensions.cs
    ├── Repositories/                    # Repository implementations
    │   ├── GenericRepository.cs
    │   ├── ProductRepository.cs
    │   ├── CategoryRepository.cs
    │   ├── ProductImageRepository.cs
    │   ├── CategoryImageRepository.cs
    │   └── UnitOfWork.cs
    ├── Services/                        # Infrastructure services
    │   └── ImageManagementService.cs
    ├── Settings/
    │   ├── FileStorageSettings.cs
    │   └── FileNamingStrategy.cs
    └── InfrastructureRegisteration.cs   # DI registration

```

### Design Patterns

- **Clean Architecture** – Dependency inversion, separation of concerns
- **Repository Pattern** – Data access abstraction
- **Unit of Work Pattern** – Transaction management
- **DTO Pattern** – Data transfer and validation
- **Specification Pattern** – Query filtering and sorting
- **Dependency Injection** – Loose coupling
- **CQRS (Future)** – Command Query Responsibility Segregation

---

## 📊 Database Schema

### Completion Status: 18% (5/28 Configurations)

```
┌────────────────────────────────────────────────────────┐
│                 ENTITY CONFIGURATIONS                   │
├────────────────────────────────────────────────────────┤
│                                                          │
│  Category Domain:  ████████████████████  100% (2/2) ✅  │
│  Product Domain:   ███░░░░░░░░░░░░░░░░   29% (2/7) 🔄  │
│  Order Domain:     ███░░░░░░░░░░░░░░░░   25% (1/4) 🔄  │
│  Cart Domain:      ░░░░░░░░░░░░░░░░░░░    0% (0/3) ❌  │
│  User Domain:      ░░░░░░░░░░░░░░░░░░░    0% (0/3) ❌  │
│  Other Domains:    ░░░░░░░░░░░░░░░░░░░    0% (0/9) ❌  │
│                                                          │
│  TOTAL:            ████░░░░░░░░░░░░░░░   18% (5/28)     │
│                                                          │
└────────────────────────────────────────────────────────┘
```

### Complete Entity List (28 Tables)

#### ✅ Configured Entities (5)
1. **Category** - Product categories
2. **CategoryImage** - Category images
3. **Product** - Main products
4. **ProductImage** - Product photos
5. **Order** - Customer orders

#### 🔴 Priority 1: MVP Required (12 configs needed)
6. **User** - User accounts
7. **Address** - Shipping/billing addresses
8. **Cart** - Shopping carts
9. **CartItem** - Cart contents
10. **ProductVariant** - Product variations (Size, Color, etc.)
11. **ProductOption** - Options (Variant selectors + Customizations)
12. **ProductOptionValue** - Option choices
13. **ProductVariantOptionValue** - Variant-option mapping
14. **OrderItem** - Order line items
15. **Payment** - Payment records
16. **Shipping** - Shipping records
17. **Brand** - Product brands

#### 🟡 Priority 2: Important (9 configs needed)
18. **CartItemOption** - Cart customizations
19. **OrderItemOption** - Order customizations
20. **OrderStatusHistory** - Order tracking
21. **ProductReview** - Customer reviews
22. **Wishlist** - Saved products
23. **Coupon** - Discount codes
24. **BrandLogo** - Brand images
25. **InventoryHistory** - Stock tracking
26. **ReturnRequest** - Return requests

#### 🟢 Priority 3: Nice to Have (2 configs needed)
27. **ReturnItem** - Return items
28. **CouponUsage** - Coupon tracking
29. **ReviewHelpfulVote** - Review voting
30. **UserSession** - Session tracking

### Entity Relationships

```
┌──────────────────────────────────────────────────────────┐
│                  CORE RELATIONSHIPS                       │
├──────────────────────────────────────────────────────────┤
│                                                            │
│  User (1) ──────────< Orders (N)                          │
│  User (1) ──────────< Reviews (N)                         │
│  User (1) ──────────< Wishlists (N)                       │
│  User (1) ──────────< Addresses (N)                       │
│  User (1) ──────────○ Cart (1)                            │
│                                                            │
│  Category (1) ──────< Products (N)                        │
│  Category (1) ──────< CategoryImages (N)                  │
│  Category (1) ──────< Category (N)  [Self-Reference]      │
│                                                            │
│  Product (1) ────────< ProductImages (N)                  │
│  Product (1) ────────< ProductVariants (N) ⭐             │
│  Product (1) ────────< ProductOptions (N) ⭐              │
│  Product (1) ────────< Reviews (N)                        │
│                                                            │
│  ProductOption (1) ──< ProductOptionValues (N) ⭐         │
│  ProductVariant (1) ─< VariantOptionValues (N) ⭐         │
│  ProductOptionValue (1) ─< VariantOptionValues (N) ⭐     │
│                                                            │
│  Cart (1) ───────────< CartItems (N)                      │
│  CartItem (1) ───────< CartItemOptions (N) ⭐             │
│  ProductVariant (1) ─< CartItems (N) ⭐ [REQUIRED]        │
│                                                            │
│  Order (1) ──────────< OrderItems (N)                     │
│  Order (1) ──────────< OrderStatusHistory (N)             │
│  OrderItem (1) ──────< OrderItemOptions (N) ⭐            │
│  ProductVariant (1) ─< OrderItems (N)                     │
│                                                            │
│  Brand (1) ──────────< Products (N)                       │
│  Coupon (1) ─────────< CouponUsage (N)                    │
│                                                            │
└──────────────────────────────────────────────────────────┘

⭐ = New variant selector system
```

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


## 📚 API Documentation


### 📖 Complete API Reference

For complete API documentation including:
- All 144 endpoint specifications
- Request/response examples
- Authentication guide
- Error handling
- Pagination details
- Rate limiting
- Variant selector system

**See:** [API_DOCUMENTATION.md](docs/api/README.md)

### Quick Examples

#### List Products with Filters
```bash
GET /api/v1/products?search=laptop&categoryId=1&minPrice=500&maxPrice=2000&sortBy=priceDesc&pageNumber=1&pageSize=20
```

#### Get Product Details
```bash
GET /api/v1/products/gaming-laptop-pro
```

#### Add to Cart (v2.0 - variant required)
```bash
POST /api/v1/cart/items
Authorization: Bearer {token}

{
  "productId": 1,
  "variantId": 3,
  "quantity": 2,
  "options": [
    {
      "optionId": 4,
      "value": "John's Laptop"
    }
  ]
}
```

### API Features

- 🔐 **JWT Authentication** - Secure token-based auth
- 📄 **Pagination** - Efficient data retrieval
- 🔍 **Advanced Filtering** - Search, filter, sort
- ⚡ **Rate Limiting** - 100 req/min (authenticated)
- 🎨 **Variant System** - Dynamic product variations
- 🛒 **Guest Cart** - Shop without account
- 📊 **Admin Analytics** - Comprehensive reports

### Response Format

All responses follow a consistent structure:

```json
{
  "success": true,
  "data": {
    // Response data
  },
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalRecords": 98
  }
}
```

### Error Handling

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Request validation failed",
    "details": [
      "Product name is required",
      "Price must be greater than 0"
    ]
  }
}
```

---

## ⚙️ Configuration

### Application Settings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;"
  },
  "FileSettings": {
    "MaxFileSizeMB": 5,
    "AllowedExtensions": [".jpg", ".jpeg", ".png", ".gif"],
    "ImagePath": "wwwroot/Images"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```


---

## 🤝 Contributing

Contributions welcome! Please follow:

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

---

## 📝 License

MIT License - see [LICENSE](LICENSE)

---

## 👤 Author

**Shrouq Ouda**
- GitHub: [@ShroukOuda](https://github.com/ShroukOuda)

---
**Made with ❤️ using ASP.NET Core 9**