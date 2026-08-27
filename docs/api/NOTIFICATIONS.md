# Notifications API

## Overview

The notifications module exposes the user notification feed and user-level notification preference management.

## Endpoints

### Notification Feed

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/Notifications/get-for-user/{userId}` | Get paginated notifications for a user |
| `GET` | `/api/Notifications/get-unread/{userId}` | Get unread notifications for a user |
| `GET` | `/api/Notifications/unread-count/{userId}` | Get unread notification count |
| `POST` | `/api/Notifications/mark-as-read/{userId}/{notificationId}` | Mark one notification as read |
| `POST` | `/api/Notifications/mark-all-as-read/{userId}` | Mark all notifications as read |

### Notification Preferences

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/Notifications/preferences/{userId}` | Get all preferences for a user |
| `POST` | `/api/Notifications/preferences/{userId}/update` | Update a single notification preference |
| `POST` | `/api/Notifications/preferences/{userId}/save` | Save a batch of preferences |
| `POST` | `/api/Notifications/preferences/{userId}/turn-off-all` | Disable all notification preferences |
| `GET` | `/api/Notifications/preferences/{userId}/is-enabled/{type}` | Check whether a notification type is enabled |

## Example Payloads

### Update Preference

```json
{
  "type": 0,
  "channel": 1,
  "isEnabled": true
}
```

### Save Multiple Preferences

```json
{
  "preferences": [
    { "type": 0, "channel": 0, "isEnabled": true },
    { "type": 3, "channel": 1, "isEnabled": false }
  ]
}
```

## Supported Notification Types

- `SecurityAlert`
- `LoginFromNewDevice`
- `PasswordChanged`
- `NewProduct`
- `BackInStock`
- `OrderPlaced`
- `OrderShipped`
- `OrderDelivered`
- `OrderCancelled`
- `Promotion`

## Supported Channels

- `InApp`
- `Email`
