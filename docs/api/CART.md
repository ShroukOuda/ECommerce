# 🛒 Shopping Cart API

Shopping cart management for guest and authenticated users.

**Total Endpoints:** 9  
**Status:** 📋 All Planned

---

## Overview

The cart system supports both:
- **Guest carts** - Identified by session/cookie
- **User carts** - Persisted to database
- **Cart merging** - Guest cart merged on login

⚠️ **Important (v2.0):** `variantId` is now **REQUIRED** when adding items!

---

## Table of Contents

- [Cart Endpoints](#cart-endpoints) (9 - All Optional Auth)
- [Cart Workflow](#cart-workflow)
- [Examples](#examples)

---

## Cart Endpoints

All cart endpoints support **Optional Authentication** 🟡:
- Works for **guest users** (no token)
- Works for **authenticated users** (with token)

### 7.1 Get Cart

```http
GET /api/v1/cart
Authorization: Bearer {token} (optional)
```

**Success Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "cartId": "guest-abc123",
    "userId": null,
    "items": [
      {
        "id": 1,
        "productId": 1,
        "productName": "Gaming Laptop Pro",
        "productSlug": "gaming-laptop-pro",
        "variantId": 3,
        "variantName": "16GB + 512GB + Gray",
        "sku": "LAP-16-512-GRAY",
        "quantity": 2,
        "unitPrice": 1299.00,
        "subtotal": 2598.00,
        "image": "/images/products/laptop-gray.jpg",
        "stockStatus": "in_stock",
        "stockQuantity": 15,
        "customizations": [
          {
            "optionId": 4,
            "optionName": "Engraving",
            "value": "John's Laptop",
            "priceAdjustment": 25.00
          }
        ]
      }
    ],
    "summary": {
      "itemCount": 2,
      "subtotal": 2598.00,
      "discountAmount": 259.80,
      "couponCode": "SAVE10",
      "taxAmount": 187.06,
      "shippingCost": 0.00,
      "total": 2525.26
    }
  }
}
```

---

### 7.2 Add Item to Cart

⚠️ **Breaking Change in v2.0:** `variantId` is now REQUIRED!

```http
POST /api/v1/cart/items
Authorization: Bearer {token} (optional)
Content-Type: application/json
```

**Request Body:**
```json
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

**Success Response (201 Created):**
```json
{
  "success": true,
  "data": {
    "cartItemId": 1,
    "quantity": 2,
    "subtotal": 2598.00,
    "cart": {
      "itemCount": 2,
      "total": 2525.26
    }
  },
  "message": "Item added to cart"
}
```

**Error Response (422 - Insufficient Stock):**
```json
{
  "success": false,
  "error": {
    "code": "INSUFFICIENT_STOCK",
    "message": "Only 5 items available",
    "details": [
      "Requested: 10",
      "Available: 5"
    ]
  }
}
```

---

### 7.3 Update Cart Item

```http
PUT /api/v1/cart/items/:id
Authorization: Bearer {token} (optional)
Content-Type: application/json
```

**Request Body:**
```json
{
  "quantity": 3
}
```

---

### 7.4 Remove Cart Item

```http
DELETE /api/v1/cart/items/:id
Authorization: Bearer {token} (optional)
```

---

### 7.5 Clear Cart

```http
DELETE /api/v1/cart/clear
Authorization: Bearer {token} (optional)
```

---

### 7.6 Get Cart Count

```http
GET /api/v1/cart/count
Authorization: Bearer {token} (optional)
```

**Success Response:**
```json
{
  "success": true,
  "data": {
    "count": 5
  }
}
```

---

### 7.7 Apply Coupon

```http
POST /api/v1/cart/coupon
Authorization: Bearer {token} (optional)
Content-Type: application/json
```

**Request Body:**
```json
{
  "couponCode": "SAVE10"
}
```

---

### 7.8 Remove Coupon

```http
DELETE /api/v1/cart/coupon
Authorization: Bearer {token} (optional)
```

---

### 7.9 Merge Guest Cart

Merge guest cart with user cart on login.

```http
POST /api/v1/cart/merge
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "guestCartId": "guest-abc123"
}
```

---

## Cart Workflow

### Guest Shopping Flow

```
1. Browse products (no auth)
2. Add to cart → Creates guest cart (session-based)
3. Continue shopping
4. Proceed to checkout
5. Option A: Guest checkout
   Option B: Login → Cart merged
```

### Authenticated Shopping Flow

```
1. Login → Get user cart
2. Add to cart → Updates user cart in database
3. Cart persists across sessions
4. Checkout
```

### Cart Merge on Login

```
Before Login:
  Guest Cart: [Laptop, Mouse]
  User Cart: [Keyboard]

After Login:
  Merged Cart: [Laptop, Mouse, Keyboard]
```

---

## Examples

### Complete Shopping Flow

```bash
# 1. Add item to cart (guest)
curl -X POST http://localhost:8080/api/v1/cart/items \
  -H "Content-Type: application/json" \
  -d '{
    "productId": 1,
    "variantId": 3,
    "quantity": 1
  }'

# 2. Get cart
curl -X GET http://localhost:8080/api/v1/cart

# 3. Apply coupon
curl -X POST http://localhost:8080/api/v1/cart/coupon \
  -H "Content-Type: application/json" \
  -d '{"couponCode": "SAVE10"}'

# 4. Login
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "SecurePass123!"
  }'

# 5. Merge cart
curl -X POST http://localhost:8080/api/v1/cart/merge \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"guestCartId": "guest-abc123"}'

# 6. Proceed to checkout
curl -X POST http://localhost:8080/api/v1/orders/checkout \
  -H "Authorization: Bearer TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "shippingAddressId": 1,
    "paymentMethod": "credit_card"
  }'
```

---

## Related Documentation

- [Products](PRODUCTS.md) - Browse products
- [Variant Selectors](VARIANT_SELECTORS.md) - Select product variant
- [Orders](ORDERS.md) - Checkout process
- [Coupons](COUPONS.md) - Coupon validation

---

**Last Updated:** February 9, 2026