# Brands API

## Endpoints

### Get All Brands
```
GET /api/Brands/get-all
```
**Response:** `200 OK` - Array of brand objects

### Get Brand by ID
```
GET /api/Brands/get-by-id/{id}
```
| Parameter | Type | Description |
|-----------|------|-------------|
| `id` | int | Brand ID |

**Response:** `200 OK` - Brand object

### Add Brand
```
POST /api/Brands/add
```
**Body:**
```json
{
  "name": "string",
  "description": "string",
  "website": "string",
  "logoUrl": "string"
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Brand added successfully" }`

### Update Brand
```
PUT /api/Brands/update
```
**Body:**
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "website": "string",
  "logoUrl": "string"
}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Brand updated successfully" }`

### Delete Brand
```
DELETE /api/Brands/delete/{id}
```
**Response:** `200 OK` - `{ "statusCode": 200, "message": "Brand deleted successfully" }`
