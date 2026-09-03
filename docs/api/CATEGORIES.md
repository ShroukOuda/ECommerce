# Categories API

Category browsing and category image management endpoints.

**Total Endpoints:** 11

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/categories` | List all categories |
| `GET` | `/api/v1/categories/{id}` | Get a category by ID |
| `POST` | `/api/v1/categories` | Create a category |
| `POST` | `/api/v1/categories/{categoryId}/images` | Upload a category image |
| `GET` | `/api/v1/categories/{categoryId}/images` | List category images |
| `GET` | `/api/v1/categories/{categoryId}/images/sub-type` | Get a category image by subtype |
| `GET` | `/api/v1/categories/{categoryId}/images/{id}` | Get a category image by ID |
| `PUT` | `/api/v1/categories/{id}` | Update a category |
| `DELETE` | `/api/v1/categories/{id}` | Delete a category |
| `DELETE` | `/api/v1/categories/{categoryId}/images/{photoId}` | Delete a category image |
| `DELETE` | `/api/v1/categories/{categoryId}/images` | Delete all category images |
