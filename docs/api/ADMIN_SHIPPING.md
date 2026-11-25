# Admin Shipping API

Admin shipping management endpoints.

**Total Endpoints:** 5

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/admin/shipping` | List all shipping records |
| `GET` | `/api/v1/admin/shipping/{id}` | Get a shipping record by ID |
| `GET` | `/api/v1/admin/shipping/order/{orderId}` | Get shipping records for an order |
| `POST` | `/api/v1/admin/shipping` | Create a shipping record |
| `PUT` | `/api/v1/admin/shipping/{id}` | Update a shipping record |

## Rules

The whole order ships together: one shipment per order.

- **Create** requires the order to be `Processing` with a `Paid` payment, and no other shipment for the order unless the earlier ones are `Failed`. `AddressId` must be the order's shipping address (when the order has one). New shipments start as `Pending`.
- **Update** validates status transitions: `Pending -> LabelCreated | InTransit`, `LabelCreated -> InTransit`, `InTransit -> OutForDelivery | Delivered | Failed`, `OutForDelivery -> Delivered | Failed`. `Delivered`, `Failed` and `Returned` are final.
- `ShippedDate` is set when the shipment first leaves, `DeliveredDate` on `Delivered`.
- When a shipment becomes `Delivered`, the system sets the order to `Delivered` (closed) in the same save and records history on both. The order's `Delivered` status cannot be set by hand through `PUT /api/v1/admin/orders/{id}/status`.
- Every shipment status change is recorded in `statusHistory` (status, source, actor id, time).
