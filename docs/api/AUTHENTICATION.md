# Authentication API

JWT authentication and account lifecycle endpoints.

**Total Endpoints:** 9

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/auth/register` | Register a new user |
| `GET` | `/api/v1/auth/confirm-email` | Confirm an email address |
| `POST` | `/api/v1/auth/login` | Login and receive tokens |
| `POST` | `/api/v1/auth/forgot-password` | Request a password reset |
| `POST` | `/api/v1/auth/reset-password` | Reset a password |
| `POST` | `/api/v1/auth/resend-confirmation-email` | Resend confirmation email |
| `POST` | `/api/v1/auth/refresh` | Refresh the access token |
| `POST` | `/api/v1/auth/logout` | Revoke a refresh token |
| `POST` | `/api/v1/auth/logout-all` | Revoke all sessions |
