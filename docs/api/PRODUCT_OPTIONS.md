# Product Options API

Product option and option value management endpoints.

**Total Endpoints:** 7

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/product-options/product/{productId}` | Get product options for a product |
| `GET` | `/api/v1/product-options/{id}` | Get a product option by ID |
| `POST` | `/api/v1/product-options` | Create a product option |
| `PUT` | `/api/v1/product-options/{id}` | Update a product option |
| `DELETE` | `/api/v1/product-options/{id}` | Delete a product option |
| `POST` | `/api/v1/product-options/{optionId}/values` | Add an option value |
| `DELETE` | `/api/v1/product-options/{optionId}/values/{valueId}` | Delete an option value |
