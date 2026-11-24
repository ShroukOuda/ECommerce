# Swagger guest cart and checkout settings

Open Swagger on the running API. In **Authorize** there are two independent inputs:

- **GuestCartToken**: paste the raw `data.guestToken` returned once by `POST /api/v1/cart/session`. No `Bearer` prefix. Swagger sends it as `X-Cart-Token` only for cart operations.
- **Bearer**: paste the login response's `data.accessToken`, without the `Bearer` prefix. Never use a Stripe API key here.

## Guest first, then sign in

1. Clear both authorization entries to start as a guest. Create a cart session and keep `data.cartId` and `data.guestToken` privately.
2. Authorize **GuestCartToken** only. Add a product/variant to that cart. For `GET /api/v1/cart`, provide its `cartId` query value. Without `cartId`, that action looks for an account cart.
3. Register, confirm email, then log in. Keep the original cart ID and guest token.
4. Authorize **Bearer** with the access token, leaving **GuestCartToken** authorized too.
5. Execute `POST /api/v1/cart/{id}/claim` with the original cart ID. Swagger sends both JWT and `X-Cart-Token`. Registration alone does not claim the cart.
6. After claim succeeds, log out **GuestCartToken** in the Authorize dialog. Keep JWT. The old guest token is invalidated. Read the same cart by ID to check its items remain.
7. Create an order from that cart, then create its checkout session and open the returned URL. Verify signed webhook delivery and read payment `Paid` and order `Confirmed`; a success redirect alone proves neither.

Keep tokens private. Authorization persistence was not enabled. Clear them when finished. Existing authorization checks still run on the server; Swagger documentation is not a permission grant.

## One backend currency

New orders read the backend setting `Stripe:Currency` through the existing Stripe settings binding. Default remains `usd`. Restart the API after changing configuration. Request currency is ignored; the customer does not choose the currency. Order/payment records keep their saved currency when settings change. Numeric catalog prices are not converted, and there is no FX service. Review prices before changing currency.

Checkout converts to Stripe minor units for supported card presentment currencies. Whole-unit currencies reject fractional totals, not round them. ISK/UGX use Stripe's special whole-unit, x100 API representation. Account/payment-method currency availability remains Stripe's decision. See https://docs.stripe.com/currencies . No schema change or migration is needed for this change.
