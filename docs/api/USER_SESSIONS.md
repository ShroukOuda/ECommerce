# User Sessions API

Session management endpoints for authenticated users and administrators.

**Total Endpoints:** 5

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/user-sessions/active` | Get the current user's active sessions |
| `GET` | `/api/v1/user-sessions/all` | Get all sessions for the current user |
| `GET` | `/api/v1/user-sessions/user/{userId}` | Get a user's sessions as admin |
| `DELETE` | `/api/v1/user-sessions/{sessionId}` | Revoke a single session |
| `DELETE` | `/api/v1/user-sessions/revoke-all` | Revoke all sessions for the current user |
