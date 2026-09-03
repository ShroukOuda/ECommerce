# Brands API

Brand management and brand logo endpoints.

**Total Endpoints:** 11

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/brands` | List all brands |
| `GET` | `/api/v1/brands/{id}` | Get a brand by ID |
| `POST` | `/api/v1/brands` | Create a brand |
| `PUT` | `/api/v1/brands/{id}` | Update a brand |
| `DELETE` | `/api/v1/brands/{id}` | Delete a brand |
| `POST` | `/api/v1/brands/{brandId}/logos` | Upload a brand logo |
| `GET` | `/api/v1/brands/{brandId}/logos` | List brand logos |
| `GET` | `/api/v1/brands/{brandId}/logos/{subType}` | Get a logo by subtype |
| `GET` | `/api/v1/brands/{brandId}/logos/{id}` | Get a logo by ID |
| `DELETE` | `/api/v1/brands/{brandId}/logos/{id}` | Delete a brand logo |
| `DELETE` | `/api/v1/brands/{brandId}/logos` | Delete all brand logos |
