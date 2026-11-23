# Cart API

Guests may shop without registration. Checkout and claim require authentication. Frontend redirects to login/registration on a checkout 401, then claims the existing guest cart. Backend does not navigate frontend pages.

| Method | Endpoint | Identity |
|---|---|---|
| POST | `/api/v1/cart/session` | Optional account, anonymous creates capability |
| GET | `/api/v1/cart?cartId={id}` | Owner token or X-Cart-Token for that guest cart |
| GET | `/api/v1/cart` | Authenticated account active cart |
| POST | `/api/v1/cart` | Add item, requires accessible active unexpired cart |
| PUT | `/api/v1/cart/{itemId}` | Update quantity, checks parent cart ownership/capability |
| DELETE | `/api/v1/cart/{cartId}/items/{itemId}` | Remove an item from an accessible cart |
| DELETE | `/api/v1/cart/{cartId}` | Clear accessible cart |
| POST | `/api/v1/cart/{cartId}/claim` | Authenticated account plus valid X-Cart-Token |

Anonymous session creation returns `{ cartId, guestToken, expiresAt }` inside the normal data envelope. Store the raw token as a secret capability and send it in `X-Cart-Token`, never query strings or logs. Only a SHA-256 hash is persisted, not the raw token. Existing legacy empty/raw guest tokens do not grant access and must not be used as fallback.

Claim keeps cart contents, assigns current account owner and clears guest capability. Existing account carts are not overwritten or automatically merged. If multiple account carts exist, clients should use explicit claimed cartId; the legacy account active-cart lookup is not a merge selector. Merge UX/policy is future work. Secure token persistence, XSS defense and a frontend/server-session design remain frontend responsibilities. All mutation quantities must be positive. Converted/expired carts cannot be mutated.

Concurrent claim/checkout needs the unresolved SQL transaction gate in [PHASE0-REMEDIATION.md](../PHASE0-REMEDIATION.md). Do not deploy while that gate remains open.
