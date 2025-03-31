# Weather API Example

This is an example .NET 8 API that demonstrates how to authenticate with the AuthServer project.

## Overview

This API provides weather forecasts and requires JWT authentication. It shows how to:

1. Validate JWT tokens issued by the AuthServer
2. Use authorization policies to check for specific scopes
3. Extract user claims from the authenticated token
4. Configure Swagger to support JWT authentication

## Getting Started

### Prerequisites

- .NET 8 SDK
- Running instance of the AuthServer project

### Configuration

The JWT authentication is configured in `appsettings.json`:

```json
"Jwt": {
  "Issuer": "AuthServer",
  "Audience": "AuthClients",
  "JwksUrl": "http://localhost:5143/.well-known/jwks.json"
}
```

Make sure the `Issuer` and `Audience` values match those configured in the AuthServer.

### Running the API

```bash
dotnet run
```

The API will be available at http://localhost:5006.

## Authentication Flow

1. Obtain a JWT token from the AuthServer using one of its authentication endpoints.
2. Include the token in API requests in the Authorization header:
   ```
   Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

## API Endpoints

All endpoints require authentication.

### GET /api/weatherforecast
Returns a 5-day weather forecast.
Requires: Basic authentication

### GET /api/weatherforecast/{id}
Returns a detailed forecast for a specific day.
Requires: The "read" scope

### PUT /api/weatherforecast/{id}
Updates a forecast for a specific day.
Requires: The "write" scope

## Testing with Swagger

1. Open the Swagger UI at `/swagger`
2. Click the "Authorize" button
3. Enter your JWT token in the format: `Bearer your-token-here`
4. All API requests will include the token

## Security Notes

- The API validates tokens using the public key from the AuthServer's JWKS endpoint
- It verifies the token's issuer, audience, and expiration
- Different endpoints require different scopes
- The API never sees or handles user credentials
