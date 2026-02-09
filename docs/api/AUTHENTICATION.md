# 🔐 Authentication API

Complete authentication and user management endpoints.

---

## Overview

The API uses **JWT (JSON Web Token)** based authentication. Tokens are valid for 1 hour and can be refreshed using refresh tokens.

**Total Endpoints:** 8  
**Status:** 📋 All Planned

---

## Table of Contents

- [Public Endpoints](#public-endpoints)
    - [1.1 Register User](#11-register-user)
    - [1.2 Login](#12-login)
    - [1.3 Forgot Password](#13-forgot-password)
    - [1.4 Reset Password](#14-reset-password)
- [User Endpoints](#user-endpoints)
    - [1.5 Get Profile](#15-get-profile)
    - [1.6 Update Profile](#16-update-profile)
    - [1.7 Change Password](#17-change-password)
    - [1.8 Logout](#18-logout)

---

## Public Endpoints

### 1.1 Register User

Register a new user account.

```http
POST /api/v1/auth/register
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890"
}
```

**Validation Rules:**

| Field | Rules |
|-------|-------|
| `email` | Required, valid email format, unique, max 100 chars |
| `password` | Required, min 8 chars, must contain uppercase, lowercase, number, special character |
| `firstName` | Required, 2-50 characters, letters only |
| `lastName` | Required, 2-50 characters, letters only |
| `phoneNumber` | Optional, valid phone format, 10-15 digits |

**Success Response (201 Created):**

```json
{
  "success": true,
  "data": {
    "userId": 1,
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "emailVerified": false
  },
  "message": "Registration successful. Please check your email to verify your account."
}
```

**Error Responses:**

```json
// 400 - Validation Error
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Request validation failed",
    "details": [
      "Password must contain at least one uppercase letter",
      "Password must contain at least one special character"
    ]
  }
}

// 409 - Email Already Exists
{
  "success": false,
  "error": {
    "code": "EMAIL_EXISTS",
    "message": "An account with this email already exists"
  }
}
```

**Example (cURL):**

```bash
curl -X POST http://localhost:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890"
  }'
```

---

### 1.2 Login

Authenticate user and receive JWT token.

```http
POST /api/v1/auth/login
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com",
  "password": "SecurePass123!"
}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZW1haWwiOiJ1c2VyQGV4YW1wbGUuY29tIiwicm9sZSI6IkN1c3RvbWVyIiwiaWF0IjoxNzA3NDg2MDAwLCJleHAiOjE3MDc0ODk2MDB9.signature",
    "refreshToken": "abc123def456ghi789...",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "user": {
      "id": 1,
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Customer",
      "emailVerified": true,
      "profileImage": null
    }
  }
}
```

**Token Claims:**

```json
{
  "sub": "1",
  "email": "user@example.com",
  "role": "Customer",
  "iat": 1707486000,
  "exp": 1707489600
}
```

**Error Responses:**

```json
// 401 - Invalid Credentials
{
  "success": false,
  "error": {
    "code": "INVALID_CREDENTIALS",
    "message": "Invalid email or password"
  }
}

// 403 - Account Locked
{
  "success": false,
  "error": {
    "code": "ACCOUNT_LOCKED",
    "message": "Your account has been locked due to too many failed login attempts. Please try again in 30 minutes."
  }
}

// 403 - Email Not Verified
{
  "success": false,
  "error": {
    "code": "EMAIL_NOT_VERIFIED",
    "message": "Please verify your email address before logging in"
  }
}
```

**Example (cURL):**

```bash
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "password": "SecurePass123!"
  }'
```

**Using the Token:**

Once you receive the access token, include it in the `Authorization` header for all authenticated requests:

```http
GET /api/v1/orders
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

### 1.3 Forgot Password

Request a password reset email.

```http
POST /api/v1/auth/forgot-password
Content-Type: application/json
```

**Request Body:**

```json
{
  "email": "user@example.com"
}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "message": "If an account with that email exists, we've sent password reset instructions."
}
```

**Note:** For security, this endpoint always returns success even if email doesn't exist.

**Example (cURL):**

```bash
curl -X POST http://localhost:8080/api/v1/auth/forgot-password \
  -H "Content-Type: application/json" \
  -d '{"email": "john.doe@example.com"}'
```

---

### 1.4 Reset Password

Reset password using token from email.

```http
POST /api/v1/auth/reset-password
Content-Type: application/json
```

**Request Body:**

```json
{
  "token": "abc123def456ghi789...",
  "newPassword": "NewSecurePass123!"
}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "message": "Password has been reset successfully. You can now log in with your new password."
}
```

**Error Responses:**

```json
// 400 - Invalid Token
{
  "success": false,
  "error": {
    "code": "INVALID_TOKEN",
    "message": "Password reset token is invalid or has expired"
  }
}

// 400 - Weak Password
{
  "success": false,
  "error": {
    "code": "WEAK_PASSWORD",
    "message": "Password does not meet security requirements",
    "details": [
      "Password must contain at least one uppercase letter",
      "Password must contain at least one number"
    ]
  }
}
```

---

## User Endpoints

All endpoints in this section require authentication (Bearer token).

### 1.5 Get Profile

Get current user's profile information.

```http
GET /api/v1/auth/profile
Authorization: Bearer {token}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "role": "Customer",
    "emailVerified": true,
    "profileImage": "/images/users/1/profile.jpg",
    "addresses": [
      {
        "id": 1,
        "type": "shipping",
        "isDefault": true,
        "firstName": "John",
        "lastName": "Doe",
        "street": "123 Main St",
        "apartment": "Apt 4B",
        "city": "New York",
        "state": "NY",
        "zipCode": "10001",
        "country": "USA",
        "phone": "+1234567890"
      },
      {
        "id": 2,
        "type": "billing",
        "isDefault": false,
        "firstName": "John",
        "lastName": "Doe",
        "street": "456 Oak Ave",
        "city": "Brooklyn",
        "state": "NY",
        "zipCode": "11201",
        "country": "USA",
        "phone": "+1234567890"
      }
    ],
    "statistics": {
      "totalOrders": 12,
      "totalSpent": 3456.78,
      "averageOrderValue": 288.07,
      "wishlistItems": 5,
      "reviewsWritten": 8
    },
    "preferences": {
      "newsletter": true,
      "orderUpdates": true,
      "productRecommendations": true
    },
    "createdAt": "2025-06-15T10:00:00Z",
    "lastLoginAt": "2026-02-09T08:30:00Z"
  }
}
```

**Example (cURL):**

```bash
curl -X GET http://localhost:8080/api/v1/auth/profile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

---

### 1.6 Update Profile

Update current user's profile information.

```http
PUT /api/v1/auth/profile
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**

```json
{
  "firstName": "John",
  "lastName": "Smith",
  "phoneNumber": "+1234567890",
  "preferences": {
    "newsletter": false,
    "orderUpdates": true,
    "productRecommendations": true
  }
}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Smith",
    "phoneNumber": "+1234567890"
  },
  "message": "Profile updated successfully"
}
```

**Example (cURL):**

```bash
curl -X PUT http://localhost:8080/api/v1/auth/profile \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Smith",
    "phoneNumber": "+1234567890"
  }'
```

---

### 1.7 Change Password

Change user's password (requires current password).

```http
POST /api/v1/auth/change-password
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**

```json
{
  "currentPassword": "OldSecurePass123!",
  "newPassword": "NewSecurePass456!"
}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "message": "Password changed successfully"
}
```

**Error Responses:**

```json
// 401 - Incorrect Current Password
{
  "success": false,
  "error": {
    "code": "INCORRECT_PASSWORD",
    "message": "Current password is incorrect"
  }
}

// 400 - Same Password
{
  "success": false,
  "error": {
    "code": "SAME_PASSWORD",
    "message": "New password must be different from current password"
  }
}
```

---

### 1.8 Logout

Logout user and invalidate tokens.

```http
POST /api/v1/auth/logout
Authorization: Bearer {token}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

**Note:** After logout, the access token and refresh token are invalidated and cannot be used for future requests.

---

## Token Management

### Access Token

- **Type:** JWT (JSON Web Token)
- **Expiration:** 1 hour (3600 seconds)
- **Storage:** Store securely (avoid localStorage, use httpOnly cookies or secure storage)
- **Usage:** Include in `Authorization` header as `Bearer {token}`

### Refresh Token

- **Type:** Opaque token
- **Expiration:** 30 days
- **Usage:** Use to obtain new access token when current one expires
- **Endpoint:** `POST /api/v1/auth/refresh` (to be implemented)

### Token Refresh Flow

```
1. Access token expires (401 Unauthorized)
2. Client sends refresh token to /auth/refresh
3. Server validates refresh token
4. Server issues new access token + refresh token
5. Client continues with new access token
```

---

## Security Best Practices

### Password Requirements

✅ **Enforced:**
- Minimum 8 characters
- At least 1 uppercase letter
- At least 1 lowercase letter
- At least 1 number
- At least 1 special character (!@#$%^&*)

✅ **Recommended:**
- Avoid common passwords
- Don't reuse passwords
- Use password manager

### Account Security Features

- **Rate Limiting:** 5 failed login attempts = 30 min lockout
- **Email Verification:** Required before login
- **Password Reset:** Token expires in 1 hour
- **Session Management:** Single device login (can be configured)
- **2FA:** Two-factor authentication (planned for v3.0)

---

## Error Codes Reference

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `VALIDATION_ERROR` | 400 | Request validation failed |
| `INVALID_CREDENTIALS` | 401 | Email or password incorrect |
| `UNAUTHORIZED` | 401 | Missing or invalid token |
| `EMAIL_NOT_VERIFIED` | 403 | Email verification required |
| `ACCOUNT_LOCKED` | 403 | Too many failed login attempts |
| `EMAIL_EXISTS` | 409 | Email already registered |
| `INVALID_TOKEN` | 400 | Password reset token invalid |
| `WEAK_PASSWORD` | 400 | Password doesn't meet requirements |
| `INCORRECT_PASSWORD` | 401 | Current password is wrong |
| `SAME_PASSWORD` | 400 | New password same as old |

---

## Examples

### Complete Registration Flow

```bash
# 1. Register
curl -X POST http://localhost:8080/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "password": "SecurePass123!",
    "firstName": "Jane",
    "lastName": "Smith",
    "phoneNumber": "+1987654321"
  }'

# 2. Verify email (click link in email)
# Opens: http://localhost:8080/verify-email?token=abc123

# 3. Login
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "password": "SecurePass123!"
  }'

# Response includes token:
# {
#   "accessToken": "eyJhbG...",
#   "user": { "id": 2, ... }
# }

# 4. Use token for authenticated requests
curl -X GET http://localhost:8080/api/v1/auth/profile \
  -H "Authorization: Bearer eyJhbG..."
```

### Password Reset Flow

```bash
# 1. Request password reset
curl -X POST http://localhost:8080/api/v1/auth/forgot-password \
  -H "Content-Type: application/json" \
  -d '{"email": "user@example.com"}'

# 2. Check email for reset link
# Receives: http://localhost:8080/reset-password?token=xyz789

# 3. Reset password with token
curl -X POST http://localhost:8080/api/v1/auth/reset-password \
  -H "Content-Type: application/json" \
  -d '{
    "token": "xyz789",
    "newPassword": "NewSecurePass456!"
  }'

# 4. Login with new password
curl -X POST http://localhost:8080/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "NewSecurePass456!"
  }'
```

---

## Related Documentation

- [Users API](USERS.md) - User management (admin)
- [Orders API](ORDERS.md) - View order history
- [Cart API](CART.md) - Cart merging on login

---

**Status:** 📋 All endpoints planned for Week 4  
**Last Updated:** February 9, 2026