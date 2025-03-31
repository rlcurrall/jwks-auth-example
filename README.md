# Auth Solution

This solution demonstrates JWT-based authentication with .NET 8 APIs and a React SPA. It consists of three projects:

1. **AuthServer** - An authentication server that issues JWT tokens
2. **WeatherApi** - An example API that validates tokens from the AuthServer
3. **WeatherSpa** - A React SPA that demonstrates the complete authentication flow

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022, VS Code, or JetBrains Rider

### Running the Solution

You can run all projects simultaneously:

#### Using Visual Studio:
1. Open `AuthSolution.sln`
2. Right-click the solution in Solution Explorer
3. Select "Set Startup Projects..."
4. Choose "Multiple startup projects"
5. Set both AuthServer and WeatherApi to "Start"
6. Press F5 to run
7. Run the WeatherSpa separately

#### Using .NET CLI and npm:
Run each project in a separate terminal:

```bash
# Terminal 1 - AuthServer
cd AuthServer
dotnet run

# Terminal 2 - WeatherApi
cd WeatherApi
dotnet run

# Terminal 3 - WeatherSpa
cd WeatherSpa
npm install
npm run dev
```

The SPA will automatically connect to the AuthServer and WeatherApi running on their default ports.


### Default URLs

- **AuthServer**: http://localhost:5143
- **WeatherApi**: http://localhost:5006
- **WeatherSpa**: http://localhost:5173

### Verifying the JWKS Endpoint

The WeatherApi needs to access the AuthServer's JWKS (JSON Web Key Set) endpoint to validate tokens. Ensure the AuthServer exposes this endpoint correctly:

```
http://localhost:5143/.well-known/jwks.json
```

You can verify it's working by accessing this URL in your browser after starting the AuthServer. It should return a JSON object containing the public keys used for token validation.

## Authentication Flow

### Step 1: Get a token from AuthServer

Either use the direct API endpoint:

```http
POST https://localhost:7001/api/login
Content-Type: application/json

{
  "username": "user1",
  "password": "password1",
  "tenant": "tenant1",
  "scopes": ["read", "write"]
}
```

Or use the redirect flow by navigating to:
```
https://localhost:7001/api/authorize?redirectUri=https://localhost:7002/swagger&state=random_state_value&tenant=tenant1
```

### Step 2: Use the token with WeatherApi

Add the token to requests in the Authorization header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Or in Swagger:
1. Click "Authorize" at the top
2. Enter: `Bearer your-token-here`
3. Click "Authorize"

## Testing the Complete Flow

### API-Only Flow
1. Start both API projects (AuthServer and WeatherApi)
2. Get a token from AuthServer using Swagger or an API client
3. Use the token to access protected endpoints in WeatherApi
4. Try endpoints with different scope requirements:
   - `/api/weatherforecast` - Basic authentication
   - `/api/weatherforecast/1` - Requires "read" scope
   - `/api/weatherforecast/me` - Shows all token claims

### Full Flow with SPA
1. Start all three projects (AuthServer, WeatherApi, and WeatherSpa)
2. Open the WeatherSpa in your browser (http://localhost:5173)
3. Use either the redirect login or direct credential login
4. After successful authentication, you'll be redirected to the Forecasts page:
   - View the 5-day weather forecast (simplified display)
   - See your access permissions based on the token scopes

## Demo Users

For testing, use one of these accounts:

- Regular User
  - Username: `user1`
  - Password: `password1`
  - Tenant: `tenant1`

- Admin User
  - Username: `admin`
  - Password: `adminpass`
  - Tenant: `tenant1`

## Projects Documentation

For detailed documentation on each project:

- [AuthServer README](./AuthServer/README.md)
- [WeatherApi README](./WeatherApi/README.md)
- [WeatherSpa README](./WeatherSpa/README.md)