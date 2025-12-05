# E-Commerce API

[![Status](https://img.shields.io/badge/Status-In%20Active%20Development-yellow)](https://github.com/yourusername/e-commerce-api)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

> **⚠️ Project Status**: 🚧 This project is **actively being developed**. I'm currently working on improving core features, adding comprehensive testing, and preparing for Docker deployment. APIs and features are subject to change as development progresses.

A robust RESTful E-Commerce API built with ASP.NET Core 9, following Clean Architecture principles. The API provides comprehensive product and category management with advanced features like filtering, sorting, pagination, and image handling.

## 📋 Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Usage Examples](#usage-examples)
- [Roadmap](#roadmap)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)

## ✨ Features

- **Product Management**
  - Full CRUD operations for products
  - Multiple image upload and management per product
  - Advanced filtering (search, price range, category)
  - Sorting (by name, price ascending/descending)
  - Pagination support
  - Category association

- **Category Management**
  - Create, read, update, and delete categories
  - Product count per category
  - Category-based product filtering

- **Image Management**
  - Multiple image upload per product
  - Image deletion and update
  - Organized folder structure per product
  - Automatic cleanup on product deletion

- **Architecture**
  - Clean Architecture implementation
  - Repository and Unit of Work patterns
  - AutoMapper for DTO mapping
  - Dependency Injection
  - Generic repository with specialized implementations

## 🛠 Technologies Used

- **Framework**: ASP.NET Core 9.0
- **Database**: SQL Server / Entity Framework Core
- **ORM**: Entity Framework Core
- **API Documentation**: Swagger/OpenAPI
- **Mapping**: AutoMapper
- **Architecture**: Clean Architecture
- **Patterns**: Repository Pattern, Unit of Work, Dependency Injection

## 📁 Project Structure

```
E-Commerce/
├── E-Commerece.Api/
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   └── CategoriesController.cs
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── E-Commerece.Application/
│   ├── Services/
│   │   ├── ProductService.cs
│   │   ├── CategoryService.cs
│   │   └── ImageManagementService.cs
│   ├── Interfaces/
│   │   ├── IProductService.cs
│   │   ├── ICategoryService.cs
│   │   └── IImageManagementService.cs
│   └── Mappings/
│       └── MappingProfile.cs
│
├── E-Commerece.Core/
│   ├── Entities/
│   │   ├── Product/
│   │   │   ├── Product.cs
│   │   │   └── Photo.cs
│   │   └── Category/
│   │       └── Category.cs
│   ├── DTO/
│   │   ├── AddProductDTO.cs
│   │   ├── UpdateProductDTO.cs
│   │   ├── GetProductDTO.cs
│   │   └── CategoryDTO.cs
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs
│   │   ├── IProductRepository.cs
│   │   ├── ICategoryRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Enums/
│
└── E-Commerece.Infrastructure/
    ├── Data/
    │   └── AppDbContext.cs
    ├── Repositories/
    │   ├── GenericRepository.cs
    │   ├── ProductRepository.cs
    │   ├── CategoryRepository.cs
    │   ├── PhotoRepository.cs
    │   └── UnitOfWork.cs
    └── Migrations/
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 / VS Code / Rider

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/e-commerce-api.git
   cd e-commerce-api
   ```

2. **Configure the database connection**
   
   Update the connection string in `E-Commerece.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ECommerceDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   cd E-Commerece.Infrastructure
   dotnet ef database update --startup-project ../E-Commerece.Api
   ```

   Or from the solution root:
   ```bash
   dotnet ef database update --project E-Commerece.Infrastructure --startup-project E-Commerece.Api
   ```

4. **Run the application**
   ```bash
   cd E-Commerece.Api
   dotnet run
   ```

5. **Access Swagger UI**
   
   Navigate to: `https://localhost:5001/swagger` or `http://localhost:5000/swagger`

## 📡 API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Products/get-all` | Get all products with filtering, sorting, and pagination |
| GET | `/api/Products/get-by-id/{id}` | Get a specific product by ID |
| POST | `/api/Products/add` | Create a new product with images |
| PUT | `/api/Products/update` | Update an existing product |
| DELETE | `/api/Products/delete/{id}` | Delete a product and its images |

### Categories

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Categories/get-all` | Get all categories |
| GET | `/api/Categories/get-by-id/{id}` | Get a specific category by ID |
| POST | `/api/Categories/add` | Create a new category |
| PUT | `/api/Categories/update` | Update an existing category |
| DELETE | `/api/Categories/delete/{id}` | Delete a category |

## 📝 Usage Examples

### Get All Products with Filtering and Pagination

```bash
curl -X GET "https://localhost:5001/api/Products/get-all?page=1&pageSize=10&sortBy=PriceAsc&search=laptop&minPrice=500&maxPrice=2000&categoryId=1" \
  -H "accept: application/json"
```

**Query Parameters:**
- `page` (int, default: 1): Page number
- `pageSize` (int, default: 10): Items per page
- `sortBy` (string): Sort order (`Name`, `PriceAsc`, `PriceDesc`)
- `search` (string): Search in product name or description
- `minPrice` (decimal): Minimum price filter
- `maxPrice` (decimal): Maximum price filter
- `categoryId` (int): Filter by category

**Response:**
```json
{
  "data": [
    {
      "id": 1,
      "name": "Gaming Laptop",
      "description": "High-performance gaming laptop",
      "price": 1299.99,
      "stockQuantity": 15,
      "categoryId": 1,
      "categoryName": "Electronics",
      "photos": [
        {
          "id": 1,
          "imageName": "images/1/laptop-front.jpg"
        }
      ]
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalRecords": 47
}
```

### Get Product by ID

```bash
curl -X GET "https://localhost:5001/api/Products/get-by-id/1" \
  -H "accept: application/json"
```

**Response:**
```json
{
  "id": 1,
  "name": "Gaming Laptop",
  "description": "High-performance gaming laptop with RTX 4070",
  "price": 1299.99,
  "stockQuantity": 15,
  "categoryId": 1,
  "categoryName": "Electronics",
  "photos": [
    {
      "id": 1,
      "imageName": "images/1/laptop-front.jpg"
    },
    {
      "id": 2,
      "imageName": "images/1/laptop-side.jpg"
    }
  ]
}
```

### Add New Product

```bash
curl -X POST "https://localhost:5001/api/Products/add" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Name=Wireless Mouse" \
  -F "Description=Ergonomic wireless mouse with RGB lighting" \
  -F "Price=29.99" \
  -F "StockQuantity=100" \
  -F "CategoryId=1" \
  -F "Photos=@/path/to/image1.jpg" \
  -F "Photos=@/path/to/image2.jpg"
```

**Request Body (form-data):**
- `Name` (string, required): Product name
- `Description` (string): Product description
- `Price` (decimal, required): Product price (must be > 0)
- `StockQuantity` (int): Available stock
- `CategoryId` (int, required): Category ID
- `Photos` (file[], optional): Multiple image files

**Response:**
```json
{
  "message": "Product added successfully",
  "productId": 42
}
```

### Update Product

```bash
curl -X PUT "https://localhost:5001/api/Products/update" \
  -H "accept: application/json" \
  -H "Content-Type: multipart/form-data" \
  -F "Id=1" \
  -F "Name=Gaming Laptop Pro" \
  -F "Description=Updated description" \
  -F "Price=1399.99" \
  -F "StockQuantity=20" \
  -F "CategoryId=1" \
  -F "PhotosToDelete=5" \
  -F "PhotosToDelete=6" \
  -F "NewPhotos=@/path/to/new-image.jpg"
```

**Request Body (form-data):**
- `Id` (int, required): Product ID
- `Name` (string, required): Updated name
- `Description` (string): Updated description
- `Price` (decimal, required): Updated price
- `StockQuantity` (int): Updated stock
- `CategoryId` (int, required): Category ID
- `PhotosToDelete` (int[]): Array of photo IDs to delete
- `NewPhotos` (file[]): New images to add

### Delete Product

```bash
curl -X DELETE "https://localhost:5001/api/Products/delete/1" \
  -H "accept: application/json"
```

### Get All Categories

```bash
curl -X GET "https://localhost:5001/api/Categories/get-all" \
  -H "accept: application/json"
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Electronics",
    "description": "Electronic devices and accessories",
    "productCount": 25
  },
  {
    "id": 2,
    "name": "Clothing",
    "description": "Fashion and apparel",
    "productCount": 50
  }
]
```

### Add New Category

```bash
curl -X POST "https://localhost:5001/api/Categories/add" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Home & Garden",
    "description": "Home improvement and gardening products"
  }'
```

## 🗺 Roadmap

### Phase 1 (Current - In Active Development)
- [x] Basic CRUD for Products
- [x] Basic CRUD for Categories
- [x] Image management system
- [x] Filtering and sorting
- [x] Pagination
- [ ] Unit testing implementation
- [ ] Input validation improvements
- [ ] Error handling middleware
- [ ] Logging system (Serilog)

### Phase 2 (In Progress)
- [ ] User authentication and authorization (JWT)
- [ ] User roles (Admin, Customer)
- [ ] Shopping cart functionality
- [ ] Order management
- [ ] Payment integration
- [ ] API versioning
- [ ] Rate limiting

### Phase 3 (Planned)
- [ ] Product reviews and ratings
- [ ] Wishlist functionality
- [ ] Inventory management
- [ ] Email notifications
- [ ] Advanced search with Elasticsearch
- [ ] Redis caching
- [ ] Product recommendations
- [ ] Analytics dashboard
- [ ] **Docker containerization**
- [ ] Docker Compose for multi-container setup
- [ ] CI/CD pipeline (GitHub Actions)

### Phase 4 (Future)
- [ ] Multi-language support
- [ ] Multi-currency support
- [ ] Discount and coupon system
- [ ] Shipping integration
- [ ] Mobile app API support
- [ ] GraphQL endpoint
- [ ] Real-time notifications (SignalR)
- [ ] Kubernetes deployment
- [ ] Microservices architecture migration
- [ ] Cloud deployment (Azure/AWS)

## 📸 Screenshots

> Screenshots will be added as the project progresses.

### Swagger UI
![Swagger Documentation](docs/screenshots/swagger-ui.png)

### Product List
![Product List](docs/screenshots/product-list.png)

### Product Details
![Product Details](docs/screenshots/product-details.png)

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

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

### Coding Standards
- Follow C# naming conventions
- Write XML documentation comments for public APIs
- Include unit tests for new features
- Update README.md if adding new features
- Ensure all tests pass before submitting PR

## 🐳 Docker Deployment (Coming Soon)

Docker support is planned for Phase 3. The deployment will include:

- **Dockerfile** for the API application
- **Docker Compose** configuration for multi-container setup:
  - ASP.NET Core API
  - SQL Server database
  - Redis cache (future)
- **Environment variable** configuration
- **Volume mapping** for persistent data and images
- **Health checks** and monitoring

### Planned Docker Setup

```yaml
# docker-compose.yml (Preview)
version: '3.8'

services:
  api:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=ECommerceDb;User=sa;Password=YourPassword123
    depends_on:
      - db
    volumes:
      - ./images:/app/wwwroot/images

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

### Future Deployment Commands

```bash
# Build and run with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop containers
docker-compose down
```

Stay tuned for updates! ⭐

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Your Name**
- GitHub: [@yourusername](https://github.com/yourusername)
- LinkedIn: [Your Name](https://linkedin.com/in/yourprofile)

---

## 📢 Development Updates

This project is being actively developed. Follow the repository for updates on:
- New features and improvements
- Docker deployment configuration
- Testing implementation
- Performance optimizations
- Bug fixes and enhancements

**Current Focus**: Building robust core features and preparing infrastructure for containerization.

## 🙏 Acknowledgments

- ASP.NET Core Documentation
- Clean Architecture by Robert C. Martin
- Entity Framework Core Team
- Docker Documentation

---

⭐ **If you find this project helpful or interesting, please consider giving it a star!** Your support motivates continued development.

**Project Status**: Actively maintained and under development  
**Last Updated**: December 2025
