# Products API

Product browsing and product management endpoints.

**Total Endpoints:** 12

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/products` | List products |
| `GET` | `/api/v1/products/{id}` | Get a product by ID |
| `GET` | `/api/v1/products/{productId}/similar` | Get similar products |
| `POST` | `/api/v1/products` | Create a product |
| `PUT` | `/api/v1/products/{id}` | Update a product |
| `DELETE` | `/api/v1/products/{id}` | Delete a product |
| `POST` | `/api/v1/products/{productId}/images` | Upload a product image |
| `DELETE` | `/api/v1/products/{productId}/images/{imageId}` | Delete a product image |
| `DELETE` | `/api/v1/products/{productId}/images` | Delete all product images |
| `GET` | `/api/v1/products/{productId}/images` | List product images |
| `POST` | `/api/v1/products/{productId}/subscribe-stock-alert` | Subscribe to stock alerts |
| `DELETE` | `/api/v1/products/{productId}/unsubscribe-stock-alert` | Unsubscribe from stock alerts |
