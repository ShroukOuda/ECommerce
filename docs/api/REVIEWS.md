# Reviews API

## Endpoints

### Get Reviews by Product
```
GET /api/Reviews/get-by-product/{productId}
```
**Response:** `200 OK` - Array of review objects

### Get Review by ID
```
GET /api/Reviews/get-by-id/{id}
```
**Response:** `200 OK` - Review object

### Add Review
```
POST /api/Reviews/add
```
**Body:**
```json
{
  "rating": 5,
  "title": "string",
  "productId": 0,
  "orderId": 0,
  "userId": "string"
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Review added successfully" }`

### Delete Review
```
DELETE /api/Reviews/delete/{id}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Review deleted successfully" }`
