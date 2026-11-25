# Orders API

User order management endpoints.

**Total Endpoints:** 5

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/orders` | Get the current user's orders |
| `GET` | `/api/v1/orders/{id}` | Get an order by ID |
| `POST` | `/api/v1/orders` | Create an order |
| `PUT` | `/api/v1/orders/{id}` | Update an order status |
| `DELETE` | `/api/v1/orders/{id}` | Delete an order |

## Order, payment and shipment states

Each state has one owner:

- **Order** (`orderStatus`): fulfillment decision only: `Pending`, `Confirmed`, `Processing`, `OnHold`, `Cancelled`, and `Delivered` (closed, set by the system when the shipment is delivered). Admins can set `Confirmed` (only with a `Paid` payment), `Processing`, `OnHold`, `Cancelled`. `Shipped`, `OutForDelivery`, `PartiallyShipped`, `ReadyForShipping` and `Refunded` remain in the enum for existing data but are no longer set through the order status endpoint.
- **Payment** (`Payment.Status`): money only. Payment timing is unchanged: the order is `Confirmed` when Stripe confirms payment.
- **Shipment** (`Shipping.Status`): the parcel only. `shippingStatus` and `paymentStatus` on the order are computed from the shipment and latest payment, not stored.

Each of the three keeps a history with status, source (`Customer`, `Admin`, `Stripe`, `Shipping`, `System`), actor id when there is one, optional reason, and time. Admins can pass an optional `reason` to `PUT /api/v1/admin/orders/{id}/status`.

Coupons: the discount and coupon code are saved on the order (`discountAmount`, `couponCode`) and included in `totalAmount`. Cancelling an order (customer or admin) gives the coupon use back (`UsedCount` and the per-user usage). A failed or expired payment does not cancel the order, so the coupon stays reserved for the retry.
