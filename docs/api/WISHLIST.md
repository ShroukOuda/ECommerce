# Wishlist API

## Endpoints

### Get Wishlist by User
```
GET /api/Wishlist/get-by-user/{userId}
```
**Response:** `200 OK` - Array of wishlist items

### Add to Wishlist
```
POST /api/Wishlist/add
```
**Body:**
```json
{
  "productId": 0,
  "userId": "string"
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Added to wishlist successfully" }`

### Remove from Wishlist
```
DELETE /api/Wishlist/remove/{id}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Removed from wishlist successfully" }`
