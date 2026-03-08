# Orders API

## Endpoints

### Get Orders by User
```
GET /api/Orders/get-by-user/{userId}
```
**Response:** `200 OK` - Array of order objects

### Get Order by ID
```
GET /api/Orders/get-by-id/{id}
```
**Response:** `200 OK` - Order object with items

### Create Order
```
POST /api/Orders/create
```
**Body:**
```json
{
  "userId": "string",
  "shippingAddressId": 0,
  "billingAddressId": 0,
  "notes": "string",
  "items": [
    {
      "productId": 0,
      "productVariantId": 0,
      "quantity": 1,
      "unitPrice": 0.00
    }
  ]
}
```
**Response:** `200 OK` - Created order object

### Update Order Status
```
PUT /api/Orders/update-status
```
**Body:**
```json
{
  "orderId": 0,
  "status": "Processing"
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Order status updated successfully" }`

### Delete Order
```
DELETE /api/Orders/delete/{id}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Order deleted successfully" }`
