# Notifications API

User notification feed and preference endpoints.

**Total Endpoints:** 8

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/v1/notifications` | Get the current user's notifications |
| `GET` | `/api/v1/notifications/unread` | Get unread notifications |
| `GET` | `/api/v1/notifications/unread-count` | Get unread notification count |
| `POST` | `/api/v1/notifications/{notificationId}/read` | Mark a notification as read |
| `POST` | `/api/v1/notifications/read-all` | Mark all notifications as read |
| `GET` | `/api/v1/notifications/preferences` | Get notification preferences |
| `PATCH` | `/api/v1/notifications/preferences/{preferenceId}` | Update a notification preference |
| `POST` | `/api/v1/notifications/preferences/turn-off-all` | Turn off all preferences |
