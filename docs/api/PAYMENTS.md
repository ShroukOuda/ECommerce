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

## Remediation rules and remaining gates

Catalog currency is server USD. No client-controlled currency conversion. USD amounts must be nonnegative exact cents; this checkout currently requires a positive total. Free-order fulfillment is an explicit future policy, not a silent Stripe workaround.

Completed-but-unpaid sessions move to Processing, never Paid. Signed financial identity must match persisted payment/order metadata, client reference, session, known intent, exact minor-unit amount and currency. Paid/refunded/failed/expired terminal attempt states are not resurrected by late success. A late paid event records money without reviving a cancelled/refunded/fulfilled order.

Actual new retries create a new Payment and transaction/idempotency identity. An unresolved pending operation without a saved session retries the same identity, with a conservative 23-hour stop for manual provider reconciliation. A locally open session is checked against the provider before reuse. Completed/expired remote state blocks reuse until signed webhook/reconciliation catches up. This can intentionally return 409 instead of risking a duplicate charge.

The webhook bypasses the middleware's shared-IP UI quota, but still requires signature verification. It is not protected by browser JWT or frontend redirect state. Dedicated perimeter/webhook abuse controls remain deployment work.

See [beginner guide](../PAYMENT-GUIDE.md) and [release gates](../PHASE0-REMEDIATION.md). SQL concurrency, operational reconciliation and end-to-end sandbox delivery are not proven by unit/InMemory tests. No Stripe refund call or automatic refund is implemented by these status guards.
