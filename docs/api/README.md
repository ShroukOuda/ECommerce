# 📚 API Documentation

**E-Commerce REST API - Version 1.0**

Complete API reference for the E-Commerce platform built with ASP.NET Core 9.

---

## 📋 Quick Reference

| Category | Endpoints | Documentation |
|----------|-----------|---------------|
| **Authentication** | 9 | [AUTHENTICATION.md](AUTHENTICATION.md) |
| **Products** | 12 | [PRODUCTS.md](PRODUCTS.md) |
| **Variant Selectors** | 0 | [VARIANT_SELECTORS.md](VARIANT_SELECTORS.md) |
| **Notifications** | 8 | [NOTIFICATIONS.md](NOTIFICATIONS.md) |
| **Categories** | 11 | [CATEGORIES.md](CATEGORIES.md) |
| **Brands** | 11 | [BRANDS.md](BRANDS.md) |
| **Shopping Cart** | 5 | [CART.md](CART.md) |
| **Orders** | 5 | [ORDERS.md](ORDERS.md) |
| **Reviews** | 4 | [REVIEWS.md](REVIEWS.md) |
| **Wishlist** | 3 | [WISHLIST.md](WISHLIST.md) |
| **Coupons** | 6 | [COUPONS.md](COUPONS.md) |
| **Users** | 0 | [USERS.md](USERS.md) |
| **Addresses** | 5 | [ADDRESSES.md](ADDRESSES.md) |
| **Inventory** | 2 | [INVENTORY.md](INVENTORY.md) |
| **Notification Preferences** | 4 | [NOTIFICATION_PREFERENCES.md](NOTIFICATION_PREFERENCES.md) |
| **Payments** | 3 | [PAYMENTS.md](PAYMENTS.md) |
| **Product Options** | 7 | [PRODUCT_OPTIONS.md](PRODUCT_OPTIONS.md) |
| **Product Variants** | 6 | [PRODUCT_VARIANTS.md](PRODUCT_VARIANTS.md) |
| **Returns** | 3 | [RETURNS.md](RETURNS.md) |
| **Shipping** | 3 | [SHIPPING.md](SHIPPING.md) |
| **User Sessions** | 5 | [USER_SESSIONS.md](USER_SESSIONS.md) |
| **Admin Shipping** | 5 | [ADMIN_SHIPPING.md](ADMIN_SHIPPING.md) |
| **Admin** | 0 | [ADMIN.md](ADMIN.md) |
| **Analytics** | 0 | [ANALYTICS.md](ANALYTICS.md) |
| **Homepage** | 1 | [HOMEPAGE.md](HOMEPAGE.md) |

**Total:** 120 endpoints

Pages with `0` are documentation hubs or legacy groupings without a dedicated controller in the current project.

---

## 🚀 Getting Started

### Base URL

| Environment | URL |
|-------------|-----|
| **Development** | `http://localhost:5000/api/v1` |


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
- `GET /api/v1/products/{id}` - Get product details
- `GET /api/v1/products/{productId}/similar` - Get similar products

#### [Categories](CATEGORIES.md)
Navigate product categories and hierarchies.
- `GET /api/v1/categories` - List all categories
- `GET /api/v1/categories/{id}` - Get category details

#### [Brands](BRANDS.md)
Browse brands and their products.
- `GET /api/v1/brands` - List all brands
- `GET /api/v1/brands/{id}` - Get brand details

---

### User APIs (Authentication Required)

#### [Shopping Cart](CART.md)
Manage shopping cart (works for both guest and authenticated users).
- `POST /api/v1/cart` - Add item to cart
- `GET /api/v1/cart` - Get current cart
- `PUT /api/v1/cart/{id}` - Update quantity

#### [Orders](ORDERS.md)
Place orders, track shipments, view order history.
- `GET /api/v1/orders` - List user orders
- `POST /api/v1/orders` - Create order
- `PUT /api/v1/orders/{id}` - Update order status
- `DELETE /api/v1/orders/{id}` - Delete order

#### [Reviews](REVIEWS.md)
Write and manage product reviews.
- `GET /api/v1/reviews/product/{productId}` - List reviews for a product
- `POST /api/v1/reviews` - Create review

#### [Wishlist](WISHLIST.md)
Save products for later.
- `GET /api/v1/wishlist` - Get wishlist
- `POST /api/v1/wishlist` - Add to wishlist
- `DELETE /api/v1/wishlist/{id}` - Remove from wishlist

---

### Admin APIs (Admin Role Required)

#### [Admin Dashboard](ADMIN.md)
No dedicated admin controller exists in the current project. See the related docs for the implemented admin-facing routes.

#### [Analytics](ANALYTICS.md)
No dedicated analytics controller exists in the current project. See the related docs for the implemented data and order routes.

---

## 🎯 Common Use Cases

### Use Case 1: Browse and Purchase Flow
1. [Browse products](PRODUCTS.md) - `GET /api/v1/products`
2. [View product details](PRODUCTS.md) - `GET /api/v1/products/{id}`
3. [Inspect options](PRODUCT_OPTIONS.md) - `GET /api/v1/product-options/product/{productId}`
4. [View variants](PRODUCT_VARIANTS.md) - `GET /api/v1/product-variants/product/{productId}`
5. [Add to cart](CART.md) - `POST /api/v1/cart`
6. [Checkout](ORDERS.md) - `POST /api/v1/orders`

### Use Case 2: User Registration and Login
1. [Register](AUTHENTICATION.md) - `POST /api/v1/auth/register`
2. [Login](AUTHENTICATION.md) - `POST /api/v1/auth/login`
3. [Confirm email](AUTHENTICATION.md) - `GET /api/v1/auth/confirm-email`

### Use Case 3: Product Management (Admin)
1. [Create product](PRODUCTS.md) - `POST /api/v1/products`
2. [Upload images](PRODUCTS.md) - `POST /api/v1/products/{productId}/images`
3. [Create variants](PRODUCT_VARIANTS.md) - `POST /api/v1/product-variants`

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






