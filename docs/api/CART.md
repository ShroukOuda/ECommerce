# Cart API

Shopping cart endpoints for the current user.

**Total Endpoints:** 5

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/cart` | Get the active cart |
| `POST` | `/api/v1/cart` | Add an item to the cart |
| `PUT` | `/api/v1/cart/{id}` | Update a cart item |
| `DELETE` | `/api/v1/cart/{cartId}/items/{cartItemId}` | Remove a cart item |
| `DELETE` | `/api/v1/cart/{id}` | Clear a cart |
