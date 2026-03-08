# Coupons API

## Endpoints

### Get All Coupons
```
GET /api/Coupons/get-all
```
**Response:** `200 OK` - Array of coupon objects

### Get Coupon by ID
```
GET /api/Coupons/get-by-id/{id}
```
**Response:** `200 OK` - Coupon object

### Get Coupon by Code
```
GET /api/Coupons/get-by-code/{code}
```
**Response:** `200 OK` - Coupon object

### Add Coupon
```
POST /api/Coupons/add
```
**Body:**
```json
{
  "code": "string",
  "description": "string",
  "discountType": "Percentage",
  "discountValue": 0.00,
  "minimumOrderAmount": 0.00,
  "maxUsageCount": 0,
  "startDate": "2025-01-01",
  "endDate": "2025-12-31"
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Coupon added successfully" }`

### Update Coupon
```
PUT /api/Coupons/update
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Coupon updated successfully" }`

### Delete Coupon
```
DELETE /api/Coupons/delete/{id}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Coupon deleted successfully" }`
