# Auth Server

A .NET 8 authentication server that supports JWT token generation and redirect-based authentication for SPAs.

## Development

### Code Formatting

This project uses EditorConfig for code formatting:

- `.editorconfig` - Defines coding styles, indentation, and formatting rules
- `omnisharp.json` - Additional OmniSharp configuration for C# language server

> **Important Note on Attribute Spacing**
>
> Per our coding standards, C# attributes should be written without spaces between them:
> - Correct: `[HttpPost][Authorize]` 
> - Incorrect: `[HttpPost] [Authorize]`
>
> Unfortunately, CSharpier (version 0.30.6) does not reliably respect this setting. If you're using
> CSharpier, you may need to manually fix attribute spacing after formatting. We've added the 
> appropriate EditorConfig setting (`csharp_space_between_attribute_sections = false`) and 
> OmniSharp setting (`"SpaceBetweenAttributeBlocks": false`), but some formatters may still
> insert spaces.

## Features

- JWT token generation via direct API calls
- Redirect-based authentication flow for SPAs
- User information endpoint for authenticated users
- Support for multi-tenant applications
- Scopes for fine-grained permission control
- Refresh token support with secure token rotation

## JWT Token Verification in Client APIs

This Auth Server uses asymmetric keys (RSA) for token signing, which is more secure than symmetric keys. The Auth Server holds the private key for signing tokens, while client APIs only need the public key for verification.

### .NET Client APIs

Add the JWT Bearer authentication in `Program.cs`:

```csharp
// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],     // Must match AuthServer's Issuer
            ValidAudience = builder.Configuration["Jwt:Audience"], // Must match AuthServer's Audience
            
            // Get the keys from the JWKS endpoint
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                // Fetch the JWKS from the Auth Server
                var httpClient = new HttpClient();
                var jwksJson = httpClient.GetStringAsync(
                    "https://your-auth-server-url/.well-known/jwks.json"
                ).Result;
                
                // Parse the JWKS
                var jwks = new JsonWebKeySet(jwksJson);
                return jwks.Keys;
            }
        };
    });

// Add authorization
builder.Services.AddAuthorization(options =>
{
    // Add policies for specific scopes if needed
    options.AddPolicy("ReadAccess", policy => policy.RequireClaim("scope", "read"));
    options.AddPolicy("WriteAccess", policy => policy.RequireClaim("scope", "write"));
});
```

In `appsettings.json`:
```json
{
  "Jwt": {
    "Issuer": "AuthServer",
    "Audience": "AuthClients"
  }
}
```

### API Configuration Requirements

1. **Public Key Distribution**:
   - The Auth Server exposes its public key at `/.well-known/jwks.json`
   - Client APIs fetch this public key to validate tokens
   - No need to share secret keys between services

2. **Matching Issuer and Audience**:
   - The `Issuer` and `Audience` values must match between the Auth Server and the APIs
   - These values are part of the token validation process

3. **Security Benefits**:
   - Asymmetric keys provide stronger security than symmetric keys
   - Even if a client API is compromised, attackers cannot create new tokens
   - Key rotation is simpler since only the Auth Server needs to update the private key

### Using JWT Tokens with HTTP Requests

Client applications should include the token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Applying Authorization in Controllers

```csharp
// Require authentication for all endpoints in this controller
[Authorize]
public class SecuredController : ControllerBase
{
    // This endpoint requires the "read" scope
    [HttpGet]
    [Authorize(Policy = "ReadAccess")]
    public IActionResult GetData()
    {
        return Ok(new { data = "This is protected data" });
    }
    
    // This endpoint requires both authentication and the "write" scope
    [HttpPost]
    [Authorize(Policy = "WriteAccess")]
    public IActionResult CreateData()
    {
        return Ok(new { result = "Data created successfully" });
    }
}
```

### Handling Tenant-Specific Authorization

For multi-tenant scenarios:

```csharp
// Create a policy that requires a specific tenant
services.AddAuthorization(options =>
{
    options.AddPolicy("TenantPolicy", policy =>
        policy.RequireAssertion(context =>
        {
            // Get the tenant claim from the token
            var tenantClaim = context.User.FindFirst("tenant");
            
            // Get the tenant from the request (e.g., route data, query string)
            var requestTenant = context.Resource as HttpContext?
                .Request.RouteValues["tenant"]?.ToString();
                
            // Ensure the token tenant matches the requested tenant
            return tenantClaim?.Value == requestTenant;
        }));
});
```

## API Endpoints

### Direct Authentication

```
POST /api/login
```

Request body:
```json
{
  "username": "user1",
  "password": "password1",
  "tenant": "tenant1",
  "scopes": ["read", "write"]
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2023-04-30T15:30:45Z",
  "refreshToken": "6fd9a3f8-0e2b-4870-8567-7c5b09a8f267",
  "refreshTokenExpiration": "2023-05-30T15:30:45Z"
}
```

### Refresh Token

```
POST /api/refresh
```

Request body:
```json
{
  "refreshToken": "6fd9a3f8-0e2b-4870-8567-7c5b09a8f267"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2023-04-30T15:30:45Z",
  "refreshToken": "8e7d5c2a-1f6b-4b9e-a3d2-6c8f7b9e0d4c",
  "refreshTokenExpiration": "2023-05-30T15:30:45Z"
}
```

### Logout (Revoke Token)

```
POST /api/logout
```

Request body:
```json
{
  "refreshToken": "6fd9a3f8-0e2b-4870-8567-7c5b09a8f267"
}
```

Response:
```json
{
  "message": "Token revoked successfully"
}
```

### Redirect Authentication

1. Redirect the user to:
```
GET /api/authorize?redirectUri=https://your-app.com/callback&state=random_state_value&tenant=tenant1
```

The `tenant` parameter is optional. If provided:
- The tenant field will be pre-filled and hidden on the login page
- Users only need to enter username and password

If not provided, users will need to manually enter the tenant value.

2. User completes authentication on the login page

3. User is redirected back to your application:
```
https://your-app.com/callback?token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...&state=random_state_value&refreshToken=6fd9a3f8-0e2b-4870-8567-7c5b09a8f267&expiry=2023-04-30T15:30:45Z&refreshExpiry=2023-05-30T15:30:45Z
```

### User Information

```
GET /api/me
```

Headers:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Response:
```json
{
  "userId": "1",
  "username": "user1",
  "tenant": "tenant1",
  "roles": ["User"],
  "scopes": ["read", "write"]
}
```

## Security Notes

### State Parameter

The `state` parameter in the redirect flow is a security mechanism that helps prevent cross-site request forgery (CSRF) attacks:

1. **Purpose**: The state parameter creates a binding between the client application's request and the subsequent callback, preventing attackers from injecting their own authentication responses.

2. **Implementation for SPAs**:

   ```javascript
   // When initiating the authentication flow
   const generateAuthRequest = () => {
     // Generate a cryptographically strong random string
     const state = crypto.randomUUID();

     // Store the state in sessionStorage (cleared when tab closes)
     sessionStorage.setItem('auth_state', state);

     // Redirect to auth server
     window.location.href = `https://auth-server/api/authorize?redirectUri=${encodeURIComponent(window.location.origin + '/callback')}&state=${state}`;
   };

   // When handling the callback
   const handleAuthCallback = () => {
     const urlParams = new URLSearchParams(window.location.search);
     const returnedState = urlParams.get('state');
     const storedState = sessionStorage.getItem('auth_state');

     // Verify the state matches
     if (!returnedState || returnedState !== storedState) {
       // Security error - potential CSRF attack
       console.error('State validation failed');
       return;
     }

     // State is valid, process the tokens
     const token = urlParams.get('token');
     const refreshToken = urlParams.get('refreshToken');
     const expiry = urlParams.get('expiry');
     const refreshExpiry = urlParams.get('refreshExpiry');
     
     if (token && refreshToken) {
       // Store the tokens securely
       storeAuthTokens({
         accessToken: token,
         refreshToken: refreshToken,
         expiry: new Date(expiry),
         refreshExpiry: new Date(refreshExpiry)
       });

       // Clean up state
       sessionStorage.removeItem('auth_state');

       // Redirect to application
       window.location.href = '/dashboard';
     }
   };
   ```

3. **Best Practices**:
   - Always verify the state parameter in your callback handler
   - Use sessionStorage rather than localStorage for better security
   - The state value should be a cryptographically strong random value
   - Clear the state after verification to prevent replay attacks

## Demo Users

For testing purposes, the following users are available:

- Regular User
  - Username: `user1`
  - Password: `password1`
  - Tenant: `tenant1`
  - Roles: `User`

- Admin User
  - Username: `admin`
  - Password: `adminpass`
  - Tenant: `tenant1`
  - Roles: `User`, `Admin`

## Configuration

JWT configuration is stored in `appsettings.json`:

```json
"Jwt": {
  "Issuer": "AuthServer",
  "Audience": "AuthClients",
  "ExpiryInHours": 1,
  "RefreshExpiryInDays": 30,
  "RefreshCleanupIntervalHours": 1
}
```

## Refresh Token Implementation

This authentication server implements refresh tokens with the following security features:

1. **Token Rotation**: Each time a refresh token is used, it's invalidated and a new refresh token is issued
2. **Absolute Expiration**: Refresh tokens have a configurable expiration time (default: 30 days)
3. **In-Memory Storage**: Currently uses thread-safe in-memory storage (ConcurrentDictionary)
4. **Background Cleanup**: A background service automatically removes expired tokens every `RefreshCleanupIntervalHours`
5. **One-Use Policy**: Refresh tokens can only be used once (prevents replay attacks)
6. **Revocation Support**: Tokens can be explicitly revoked through the logout endpoint
7. **Secure Integration**: Both API and redirect flows support refresh tokens

### Storage Considerations

The current implementation uses in-memory storage, which works for development but is not suitable for production environments with multiple instances or requiring persistence across restarts.

For production, replace the `RefreshTokenService` with an implementation that stores tokens in:
- Redis (recommended for stateless scaling)
- SQL database (SQL Server, PostgreSQL, etc.)
- NoSQL database (MongoDB, etc.)

## Using Refresh Tokens in Client Applications

### Token Management in SPAs

```javascript
// Refresh token logic for SPAs
const refreshAccessToken = async () => {
  const tokens = getTokensFromSecureStorage();
  
  if (!tokens || !tokens.refreshToken) {
    // No refresh token, redirect to login
    redirectToLogin();
    return;
  }
  
  try {
    const response = await fetch('https://auth-server/api/refresh', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: tokens.refreshToken })
    });
    
    if (!response.ok) {
      throw new Error('Token refresh failed');
    }
    
    const newTokens = await response.json();
    
    // Store the new tokens
    storeAuthTokens({
      accessToken: newTokens.token,
      refreshToken: newTokens.refreshToken,
      expiry: new Date(newTokens.expiration),
      refreshExpiry: new Date(newTokens.refreshTokenExpiration)
    });
    
    return newTokens.token;
  } catch (error) {
    console.error('Failed to refresh token:', error);
    // Clear tokens and redirect to login
    clearTokens();
    redirectToLogin();
  }
};

// Set up an interceptor to handle token expiration
const setupAuthInterceptor = () => {
  let refreshPromise = null;
  
  return async (request) => {
    const tokens = getTokensFromSecureStorage();
    
    // If no tokens or expired refresh token, redirect to login
    if (!tokens || new Date() > new Date(tokens.refreshExpiry)) {
      clearTokens();
      redirectToLogin();
      return;
    }
    
    // If access token expired, refresh it
    if (new Date() > new Date(tokens.expiry)) {
      // If already refreshing, wait for that to complete
      if (!refreshPromise) {
        refreshPromise = refreshAccessToken()
          .finally(() => { refreshPromise = null; });
      }
      
      try {
        const newAccessToken = await refreshPromise;
        // Update the request with the new token
        request.headers.Authorization = `Bearer ${newAccessToken}`;
      } catch (error) {
        // Error handled in refreshAccessToken
        return;
      }
    } else {
      // Token still valid, use it
      request.headers.Authorization = `Bearer ${tokens.accessToken}`;
    }
    
    return request;
  };
};
```

## Security Best Practices

1. **Store tokens securely in client applications**:
   - Never store tokens in localStorage (vulnerable to XSS)
   - Use HTTP-only, secure cookies or in-memory storage 
   - For SPAs, consider using the BFF (Backend for Frontend) pattern

2. **Short-lived access tokens**:
   - Keep access token lifetimes short (e.g., 15-60 minutes)
   - Use refresh tokens for silent renewal

3. **HTTPS only**:
   - Always use HTTPS in production
   - Set the Secure flag on all auth cookies

4. **CORS configuration**:
   - Strictly limit allowed origins
   - Be specific with allowed methods and headers

**Warning**: In production, store your JWT keys securely using Azure Key Vault, AWS KMS, HashiCorp Vault, or similar services. Never store private keys in configuration files.
