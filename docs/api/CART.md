# Cart API

## Endpoints

### Get Cart by User
```
GET /api/Cart/get-by-user/{userId}
```
**Response:** `200 OK` - Cart object with items

### Add Item to Cart
```
POST /api/Cart/add-item
```
**Body:**
```json
{
  "cartId": 0,
  "productId": 0,
  "productVariantId": 0,
  "quantity": 1,
  "unitPrice": 0.00
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Item added to cart successfully" }`

### Update Cart Item
```
PUT /api/Cart/update-item
```
**Body:**
```json
{
  "cartItemId": 0,
  "quantity": 1
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Cart item updated successfully" }`

### Remove Cart Item
```
DELETE /api/Cart/remove-item/{cartItemId}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Item removed from cart successfully" }`

### Clear Cart
```
DELETE /api/Cart/clear/{cartId}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Cart cleared successfully" }`