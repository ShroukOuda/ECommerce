# 🛍️ Products API

Product catalog management and browsing endpoints.

---

## Table of Contents

- [Public Endpoints](#public-endpoints) (10)
- [User Endpoints](#user-endpoints) (1)
- [Admin Endpoints](#admin-endpoints) (13)

---

## Public Endpoints

| Method | Endpoint                              | Description                    | Parameters                           | Status |
|--------|---------------------------------------|--------------------------------|--------------------------------------|--------|
| GET    | `/api/v1/products`                    | List all products with filters | See [Query Parameters](#query-parameters) | ✅ Implemented |
| GET    | `/api/v1/products/{slug}`             | Get product details by slug    | `slug` (string)                      | ✅ Implemented |
| GET    | `/api/v1/products/search`             | Search products with autocomplete | `q` (string, required), `limit` (int, optional, default: 10) | 📋 Planned |
| GET    | `/api/v1/products/featured`           | Get featured products          | `pageNumber` (int, optional, default: 1), `pageSize` (int, optional, default: 20) | 📋 Planned |
| GET    | `/api/v1/products/best-sellers`       | Get best-selling products      | `pageNumber` (int, optional), `pageSize` (int, optional) | 📋 Planned |
| GET    | `/api/v1/products/new-arrivals`       | Get newest products            | `pageNumber` (int, optional), `pageSize` (int, optional) | 📋 Planned |
| GET    | `/api/v1/products/hot-deals`          | Get products on sale/discount  | `pageNumber` (int, optional), `pageSize` (int, optional) | 📋 Planned |
| GET    | `/api/v1/products/top-rated`          | Get highest-rated products     | `pageNumber` (int, optional), `pageSize` (int, optional) | 📋 Planned |
| GET    | `/api/v1/products/{slug}/similar`     | Get similar/related products   | `slug` (string, required), `limit` (int, optional, default: 5) | 📋 Planned |
| POST   | `/api/v1/products/{id}/increment-view`| Track product view count       | `id` (int)                           | 📋 Planned |


## Query Parameters

**For `GET /api/v1/products` endpoint:**

| Parameter     | Type    | Required | Default | Description                  | Example Values               |
|---------------|---------|----------|---------|------------------------------|------------------------------|
| `search`      | string  | No       | -       | Search in name/description   | `"laptop"`, `"gaming mouse"` |
| `categoryId`  | integer | No       | -       | Filter by category ID        | `1`, `5`, `12`               |
| `brandId`     | integer | No       | -       | Filter by brand ID           | `2`, `8`                     |
| `minPrice`    | decimal | No       | -       | Minimum price (inclusive)    | `500.00`, `99.99`            |
| `maxPrice`    | decimal | No       | -       | Maximum price (inclusive)    | `2000.00`, `1499.99`         |
| `inStock`     | boolean | No       | `false` | Show only in-stock products  | `true`, `false`              |
| `isFeatured`  | boolean | No       | `false` | Show only featured products  | `true`, `false`              |
| `sortBy`      | string  | No       | `date`  | Sort order                   | `price`, `priceDesc`, `name`, `date`, `rating` |
| `pageNumber`  | integer | No       | `1`     | Page number (1-based)        | `1`, `2`, `10`               |
| `pageSize`    | integer | No       | `20`    | Items per page (max: 100)    | `10`, `20`, `50`, `100`      |

**Example Request:**
```
GET /api/v1/products?search=gaming&categoryId=2&minPrice=500&maxPrice=2000&sortBy=priceDesc&pageNumber=1&pageSize=20
```

**Valid `sortBy` Values:**
- `price` - Price: Low to High
- `priceDesc` - Price: High to Low
- `name` - Name: A to Z
- `nameDesc` - Name: Z to A
- `date` - Newest First (default)
- `dateDesc` - Oldest First
- `rating` - Highest Rated First
- `ratingDesc` - Lowest Rated First


**Success Response (200 OK):**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "slug": "gaming-laptop-pro",
      "name": "Gaming Laptop Pro",
      "description": "High-performance gaming laptop",
      "basePrice": 999.00,
      "priceRange": {
        "min": 999.00,
        "max": 1599.00
      },
      "categoryId": 2,
      "categoryName": "Laptops",
      "brandId": 3,
      "brandName": "TechBrand",
      "stockStatus": "in_stock",
      "rating": 4.5,
      "reviewCount": 128,
      "isFeatured": true,
      "hasVariants": true,
      "primaryImage": {
        "url": "/images/products/laptop-1.jpg",
        "alt": "Gaming Laptop Pro"
      }
    }
  ],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalRecords": 98
  }
}
```


### Get Product Details 

Get detailed product information by slug.

```http
GET /api/v1/products/:slug
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "slug": "gaming-laptop-pro",
    "name": "Gaming Laptop Pro",
    "description": "High-performance gaming laptop",
    "longDescription": "Full detailed description...",
    "basePrice": 999.00,
    "categoryId": 2,
    "brandId": 3,
    "sku": "LAP-BASE",
    "stockStatus": "in_stock",
    "rating": 4.5,
    "reviewCount": 128,
    "isFeatured": true,
    "hasVariants": true,
    "images": [
      {
        "id": 1,
        "url": "/images/products/laptop-1.jpg",
        "isPrimary": true
      }
    ],
    "specifications": {
      "Processor": "Intel Core i7",
      "RAM": "Up to 32GB",
      "Storage": "Up to 1TB SSD"
    },
    "tags": ["gaming", "laptop", "high-performance"]
  }
}
```
---

## User Endpoints

| Method | Endpoint                              | Description                    | Parameters      | Status |
|--------|---------------------------------------|--------------------------------|-----------------|-------|
| POST   | `/api/v1/products/{slug}/stock-alert` | Subscribe to stock alerts | `slug` (string) | 📋 Planned  |


### Subscribe to Stock Alert

Subscribe to receive email notification when product is back in stock.
```http
POST /api/v1/products/{slug}/stock-alert
Authorization: Bearer {token}
Content-Type: application/json
```

**Path Parameters:**
- `slug` (string, required) - Product URL slug

**Request Body:**
```json
{
  "email": "user@example.com"
}
```

**Success Response (201 Created):**
```json
{
  "success": true,
  "message": "You will be notified when this product is back in stock",
  "data": {
    "productId": 1,
    "productName": "Gaming Laptop Pro",
    "email": "user@example.com",
    "subscribedAt": "2026-02-09T10:30:00Z"
  }
}
```

**Error Responses:**
```json
// 404 - Product Not Found
{
  "success": false,
  "error": {
    "code": "PRODUCT_NOT_FOUND",
    "message": "Product with slug 'gaming-laptop-pro' not found"
  }
}

// 409 - Already Subscribed
{
  "success": false,
  "error": {
    "code": "ALREADY_SUBSCRIBED",
    "message": "You are already subscribed to stock alerts for this product"
  }
}

// 422 - Product In Stock
{
  "success": false,
  "error": {
    "code": "PRODUCT_IN_STOCK",
    "message": "This product is currently in stock"
  }
}
```

**Example (cURL):**
```bash
curl -X POST http://localhost:8080/api/v1/products/gaming-laptop-pro/stock-alert \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com"}'
```

**Notes:**
- User will receive email when product stock quantity > 0
- Each user can only subscribe once per product
- Subscription is automatically removed after notification is sent
- Product must be out of stock to subscribe

---

## Admin Endpoints

| Method | Endpoint                                             | Description                | Parameters                                             | Status |
|--------|------------------------------------------------------|----------------------------|--------------------------------------------------------|--------|
| GET    | `/api/v1/admin/products`                             | List all products (admin)  | See [Query Parameters](#query-parameters)              | ✅ Implemented |
| POST   | `/api/v1/admin/products`                             | Create product             | `CreateProductDto` (JSON)                              | ✅ Implemented |
| GET    | `/api/v1/admin/products/{id}`                        | Get product details (admin) | `id` (int)                                             | ✅ Implemented |
| PUT    | `/api/v1/admin/products/{id}`                        | Update product             | `id` (int), 'UpdateProductDto` (JSON)                  | 📋 Planned |
| DELETE | `/api/v1/admin/products/{id}`                        | Delete product             | `id` (int)                                             | 📋 Planned |
| PATCH  | `/api/v1/admin/products/{id}/status`                 |  Update product status    | `id` (int), `{ "status": "Published" }`                                          | 📋 Planned |
| POST   | `/api/v1/admin/products/bulk-update`                 | Bulk update products       | `BulkUpdateDto` (JSON)                                 | 📋 Planned |
| POST   | `/api/v1/admin/products/bulk-delete`                 | Bulk delete products       | `BulkDeleteDto` (JSON)                                 | 📋 Planned |
| POST   | `/api/v1/admin/products/{id}/images`                 | Add product image          | `id` (int), `images[]` (files),  `isMain` (bool)       | 📋 Planned |
| DELETE | `/api/v1/admin/products/{id}/images/{image_id}`      | Delete product image       | `id` (int), `imageId` (int)                            | 📋 Planned |
| POST   | `/api/v1/admin/products/{id}/images/{image_id}/main` | Set image as main          | `id` (int), `imageId` (int)                            | 📋 Planned |
| POST   | `/api/v1/admin/products/{id}/variants`               | Add product variant        | `id` (int), `variantId` (int), `CreateVariantId` (JSON) | 📋 Planned |
| PUT    | `/api/v1/admin/products/{id}/variants/{variantId}`   | Update variant             | `id` (int), `variantId` (int), `UpdateVariantId` (JSON) | 📋 Planned |
| DELETE | `/api/v1/admin/products/{id}/variants/{variantId}`   | Delete variant             | `id` (int), `variantId` (int)                          | 📋 Planned |
| GET    | `/api/v1/admin/products/stock-alert`                 | Get low-stock products     | `id` (int)                                             | 📋 Planned |
| POST   | `/api/v1/admin/products/{id}/restock`                 | Restock product     | `id` (int), `{ "quantity": 50, "reason": "Restock" }`  | 📋 Planned |
| GET    | `/api/v1/admin/products/export`                 |   Export products to CSV/Excel   | `format` (csv/excel), `categoryId`, `brandId`          | 📋 Planned |


### Create Product 

```http
POST /api/v1/admin/products
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "name": "Gaming Laptop Pro",
  "slug": "gaming-laptop-pro",
  "description": "High-performance laptop",
  "basePrice": 999.00,
  "categoryId": 2,
  "brandId": 3,
  "sku": "LAP-BASE",
  "stockQuantity": 50,
  "status": "active",
  "isFeatured": true
}
```

**Success Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "slug": "gaming-laptop-pro",
    "name": "Gaming Laptop Pro"
  },
  "message": "Product created successfully"
}
```


---

## Related Documentation

- [Variant Selectors](VARIANT_SELECTORS.md) - Product variant selection
- [Categories](CATEGORIES.md) - Product categories
- [Reviews](REVIEWS.md) - Product reviews
- [Cart](CART.md) - Adding products to cart

