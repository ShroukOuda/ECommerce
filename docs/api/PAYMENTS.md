# Payments API

Stripe Checkout is the payment flow for orders. The client creates a checkout session for an authenticated order, Stripe confirms payment through a webhook, and the API updates the internal payment record and order status.

**Total Endpoints:** 4

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/payments/checkout-session` | Create or reuse a Stripe Checkout Session for an order |
| `GET` | `/api/v1/payments/order/{orderId}` | Get payments for an order |
| `GET` | `/api/v1/payments/{id}` | Get a payment by ID |
| `POST` | `/api/v1/payments/webhook` | Receive Stripe webhook events |

## Checkout Session

Request:

```json
{
	"orderId": "00000000-0000-0000-0000-000000000001"
}
```

Response:

```json
{
	"success": true,
	"message": "Checkout session created successfully.",
	"data": {
		"paymentId": "00000000-0000-0000-0000-000000000010",
		"orderId": "00000000-0000-0000-0000-000000000001",
		"transactionId": "PAY-00000000-00000000",
		"checkoutSessionId": "cs_test_123",
		"checkoutSessionUrl": "https://checkout.stripe.com/pay/cs_test_123",
		"amount": 99.99,
		"amountInMinorUnits": 9999,
		"currency": "USD",
		"status": "Pending"
	}
}
```

## Status Model

Payments transition through `Pending`, `Processing`, `Paid`, `Failed`, `Expired`, and `Refunded`. The webhook is the source of truth for confirmation.

## Security

- The API validates order ownership before creating a checkout session.
- Payment reads are restricted to the owning user or admins.
- Webhook requests are verified with Stripe signatures.
- The client success page does not mark a payment as successful.
