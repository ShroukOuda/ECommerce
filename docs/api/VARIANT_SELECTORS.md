# Product Options & Variants API

## Product Options

### Get Options by Product
```
GET /api/ProductOptions/get-by-product/{productId}
```
**Response:** `200 OK` - Array of product option objects with values

### Get Option by ID
```
GET /api/ProductOptions/get-by-id/{id}
```
**Response:** `200 OK` - Product option object

### Add Option
```
POST /api/ProductOptions/add
```
**Body:**
```json
{
  "name": "string",
  "displayType": "Dropdown",
  "type": "VariantSelector",
  "attributeKey": "string",
  "isRequired": true,
  "priceValue": 0.00,
  "sortOrder": 0,
  "productId": 0
}
```
**Response:** `200 OK`

### Update Option
```
PUT /api/ProductOptions/update
```
**Response:** `200 OK`

### Delete Option
```
DELETE /api/ProductOptions/delete/{id}
```
**Response:** `200 OK`

### Add Option Value
```
POST /api/ProductOptions/add-value
```
**Body:**
```json
{
  "value": "string",
  "label": "string",
  "priceValue": 0.00,
  "isDefault": false,
  "sortOrder": 0,
  "imageUrl": "",
  "optionId": 0
}
```
**Response:** `200 OK`

### Delete Option Value
```
DELETE /api/ProductOptions/delete-value/{id}
```
**Response:** `200 OK`

## Product Variants

### Get Variants by Product
```
GET /api/ProductVariants/get-by-product/{productId}
```
**Response:** `200 OK` - Array of variant objects

### Get Variant by ID
```
GET /api/ProductVariants/get-by-id/{id}
```
**Response:** `200 OK` - Variant object

### Add Variant
```
POST /api/ProductVariants/add
```
**Body:**
```json
{
  "sku": "string",
  "variantName": "string",
  "size": "string",
  "color": "string",
  "material": "string",
  "priceAdjustment": 0.00,
  "stockQuantity": 0,
  "productId": 0,
  "optionValueIds": [1, 2]
}
```
**Response:** `200 OK`

### Update Variant
```
PUT /api/ProductVariants/update
```
**Response:** `200 OK`

### Delete Variant
```
DELETE /api/ProductVariants/delete/{id}
```
**Response:** `200 OK`
