# 📚 API Documentation

**E-Commerce REST API - Version 2.0**

Complete API reference for the E-Commerce platform built with ASP.NET Core 9.

---

## 📋 Quick Reference

| Category | Endpoints | Status | Documentation |
|----------|-----------|--------|---------------|
| **Authentication** | 8 | 📋 Planned | [AUTHENTICATION.md](AUTHENTICATION.md) |
| **Products** | 24 | ✅ Partial (13) | [PRODUCTS.md](PRODUCTS.md) |
| **Variant Selectors** | 6 | 📋 Planned | [VARIANT_SELECTORS.md](VARIANT_SELECTORS.md) |
| **Notifications** | 10 | ✅ Added | [NOTIFICATIONS.md](NOTIFICATIONS.md) |
| **Categories** | 12 | ✅ Partial (11) | [CATEGORIES.md](CATEGORIES.md) |
| **Brands** | 9 | 📋 Planned | [BRANDS.md](BRANDS.md) |
| **Shopping Cart** | 9 | 📋 Planned | [CART.md](CART.md) |
| **Orders** | 16 | 📋 Planned | [ORDERS.md](ORDERS.md) |
| **Reviews** | 12 | 📋 Planned | [REVIEWS.md](REVIEWS.md) |
| **Wishlist** | 6 | 📋 Planned | [WISHLIST.md](WISHLIST.md) |
| **Coupons** | 10 | 📋 Planned | [COUPONS.md](COUPONS.md) |
| **Users** | 6 | 📋 Planned | [USERS.md](USERS.md) |
| **Admin** | 16 | 📋 Planned | [ADMIN.md](ADMIN.md) |
| **Analytics** | 9 | 📋 Planned | [ANALYTICS.md](ANALYTICS.md) |
| **Homepage** | 4 | 📋 Planned | [HOMEPAGE.md](HOMEPAGE.md) |

**Total:** 144 endpoints (24 implemented, 120 planned)

---

## 🚀 Getting Started

### Base URL

| Environment | URL |
|-------------|-----|
| **Development** | `http://localhost:8080/api/v1` |
| **Staging** | `https://staging-api.yourdomain.com/api/v1` |
| **Production** | `https://api.yourdomain.com/api/v1` |

### Authentication

Most endpoints require authentication. See [AUTHENTICATION.md](AUTHENTICATION.md) for details.

**Quick Example:**
```bash
# Login
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"SecurePass123!"}'

# Use token
curl -X GET http://localhost:8080/api/v1/orders \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

## 📖 Documentation by Category

### Public APIs (No Authentication Required)

#### [Products](PRODUCTS.md)
Browse and search products, view details, get recommendations.
- `GET /api/v1/products` - List products with filters
- `GET /api/v1/products/:slug` - Get product details
- `GET /api/v1/products/featured` - Get featured products

#### [Categories](CATEGORIES.md)
Navigate product categories and hierarchies.
- `GET /api/v1/categories` - List all categories
- `GET /api/v1/categories/tree` - Get category tree

#### [Brands](BRANDS.md)
Browse brands and their products.
- `GET /api/v1/brands` - List all brands
- `GET /api/v1/brands/:slug/products` - Get brand products

---

### User APIs (Authentication Required)

#### [Shopping Cart](CART.md)
Manage shopping cart (works for both guest and authenticated users).
- `POST /api/v1/cart/items` - Add item to cart
- `GET /api/v1/cart` - Get current cart
- `PUT /api/v1/cart/items/:id` - Update quantity

#### [Orders](ORDERS.md)
Place orders, track shipments, view order history.
- `POST /api/v1/orders/checkout` - Create order from cart
- `GET /api/v1/orders` - List user orders
- `GET /api/v1/orders/:number/track` - Track order

#### [Reviews](REVIEWS.md)
Write and manage product reviews.
- `POST /api/v1/products/:slug/reviews` - Create review
- `PUT /api/v1/reviews/:id` - Update review

#### [Wishlist](WISHLIST.md)
Save products for later.
- `POST /api/v1/wishlist` - Add to wishlist
- `GET /api/v1/wishlist` - Get wishlist

---

### Admin APIs (Admin Role Required)

#### [Admin Dashboard](ADMIN.md)
Comprehensive admin functionality.
- Product management
- Order management
- User management
- Content management

#### [Analytics](ANALYTICS.md)
Business intelligence and reporting.
- Sales analytics
- Customer analytics
- Product performance
- Inventory reports

---

## 🎯 Common Use Cases

### Use Case 1: Browse and Purchase Flow
1. [Browse products](PRODUCTS.md#21-list-products) - `GET /api/v1/products`
2. [View product details](PRODUCTS.md#22-get-product-details) - `GET /api/v1/products/:slug`
3. [Select variant](VARIANT_SELECTORS.md#32-find-variant-by-selection) - `POST /api/v1/products/:slug/find-variant`
4. [Add to cart](CART.md#72-add-item-to-cart) - `POST /api/v1/cart/items`
5. [Checkout](ORDERS.md#82-checkout) - `POST /api/v1/orders/checkout`

### Use Case 2: User Registration and Login
1. [Register](AUTHENTICATION.md#11-register-user) - `POST /api/v1/auth/register`
2. [Login](AUTHENTICATION.md#12-login) - `POST /api/v1/auth/login`
3. [Get profile](AUTHENTICATION.md#13-get-profile) - `GET /api/v1/auth/profile`

### Use Case 3: Product Management (Admin)
1. [Create product](PRODUCTS.md#211-create-product) - `POST /api/v1/admin/products`
2. [Upload images](PRODUCTS.md#214-upload-product-image) - `POST /api/v1/admin/products/:id/images`
3. [Create variants](PRODUCTS.md#2111-add-product-variant) - `POST /api/v1/admin/products/:id/variants`

---

## 📝 Response Format

### Success Response

```json
{
  "success": true,
  "data": {
    // Response data here
  },
  "message": "Operation completed successfully"
}
```

### Error Response

```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "details": ["Additional detail 1", "Additional detail 2"]
  }
}
```

### Paginated Response

```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "totalRecords": 98,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

## 🔐 Authentication

All authenticated endpoints require a JWT token in the `Authorization` header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

See [AUTHENTICATION.md](AUTHENTICATION.md) for complete authentication guide.

---

## ⚡ Rate Limiting

| User Type | Limit | Window |
|-----------|-------|--------|
| **Public** | 20 requests | 1 minute |
| **Authenticated** | 100 requests | 1 minute |
| **Admin** | 200 requests | 1 minute |

Rate limit headers:
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1675941600
```

---

## 📊 HTTP Status Codes

| Code | Meaning | When Used |
|------|---------|-----------|
| `200` | OK | Successful GET, PUT, DELETE |
| `201` | Created | Successful POST |
| `204` | No Content | Successful DELETE with no body |
| `400` | Bad Request | Validation error |
| `401` | Unauthorized | Missing or invalid token |
| `403` | Forbidden | Insufficient permissions |
| `404` | Not Found | Resource not found |
| `409` | Conflict | Duplicate resource |
| `422` | Unprocessable Entity | Business logic error |
| `429` | Too Many Requests | Rate limit exceeded |
| `500` | Internal Server Error | Server error |

---

## 🔗 Additional Resources

- [Examples](examples/) - Request/response examples
- [Error Codes](ERROR_CODES.md) - Complete error reference
- [Changelog](CHANGELOG.md) - API version history
- [Migration Guide](MIGRATION_GUIDE.md) - Upgrading from v1.x

---

## 📈 API Statistics

```
Total Endpoints:        144
Implemented:            24 (17%)
In Progress:            12 (8%)
Planned:               108 (75%)

By Category:
  Products:             13/24 (54%)
  Categories:           11/12 (92%)
  Others:                0/108 (0%)

By Access Level:
  Public:               36 (25%)
  User Auth:            22 (15%)
  Optional Auth:        12 (8%)
  Admin Only:           74 (51%)
```



