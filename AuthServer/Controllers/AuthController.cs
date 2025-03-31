using System.Security.Claims;
using AuthServer.Contracts;
using AuthServer.Models;
using AuthServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Controllers;

/// <summary>
/// Authentication controller that provides endpoints for JWT token generation and redirect-based authentication
/// </summary>
[ApiController]
[Produces("application/json")]
public partial class AuthenticationController(
    IAuthenticationService authService,
    IRedirectUriValidator redirectUriValidator,
    IKeyManagementService keyManagementService,
    IRefreshTokenService refreshTokenService,
    TokenConfiguration tokenConfiguration,
    ILogger<AuthenticationController> logger
) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">The login credentials and requested scopes</param>
    /// <returns>A JWT token with the specified scopes and expiration date</returns>
    /// <response code="200">Returns the JWT token if authentication is successful</response>
    /// <response code="401">If the credentials are invalid</response>
    [HttpPost("api/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        Log.LoginAttempt(logger, request.Username, request.Tenant);

        var user = await authService.AttemptLogin(
            request.Username,
            request.Password,
            request.Tenant
        );

        if (user is null)
        {
            Log.LoginFailed(logger, request.Username, request.Tenant);
            return Unauthorized("Invalid username or password");
        }

        Log.LoginSuccess(logger, user.Username, string.Join(", ", request.Scopes));

        // Generate both access token and refresh token
        var (accessToken, refreshToken) = authService.GenerateTokenWithRefresh(
            user,
            request.Scopes
        );

        Log.TokenGenerated(
            logger,
            user.Username,
            DateTime.Now.AddHours(tokenConfiguration.ExpiryInHours)
        );

        return Ok(
            new LoginResponse
            {
                Token = accessToken,
                Expiration = DateTime.Now.AddHours(tokenConfiguration.ExpiryInHours),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresAt,
            }
        );
    }

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="request">The refresh token request</param>
    /// <returns>A new JWT token and refresh token</returns>
    /// <response code="200">Returns the new JWT token if refresh is successful</response>
    /// <response code="401">If the refresh token is invalid or expired</response>
    [HttpPost("api/refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
    {
        var (newAccessToken, newRefreshToken) = await authService.RefreshToken(
            request.RefreshToken
        );

        if (newAccessToken == null || newRefreshToken == null)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        Log.TokenRefreshed(logger, newRefreshToken.Username);

        return Ok(
            new LoginResponse
            {
                Token = newAccessToken,
                Expiration = DateTime.Now.AddHours(tokenConfiguration.ExpiryInHours),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresAt,
            }
        );
    }

    /// <summary>
    /// Revokes a refresh token, effectively logging the user out
    /// </summary>
    /// <param name="request">The refresh token to revoke</param>
    /// <returns>Success confirmation</returns>
    /// <response code="200">The token was successfully revoked</response>
    /// <response code="400">If the token couldn't be revoked</response>
    [HttpPost("api/logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshRequest request)
    {
        var result = await refreshTokenService.RevokeRefreshToken(request.RefreshToken);

        return !result
            ? BadRequest("Invalid refresh token")
            : Ok(new { Message = "Token revoked successfully" });
    }

    /// <summary>
    /// Initiates a redirect-based authentication flow
    /// </summary>
    /// <param name="request">Request containing redirect URI, state, optional tenant, and optional scopes</param>
    /// <returns>Redirects to the login page</returns>
    /// <response code="302">Redirects to the login page</response>
    /// <response code="400">If the redirect URI is invalid</response>
    [HttpGet("api/authorize")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Authorize([FromQuery] RedirectRequest request)
    {
        if (
            string.IsNullOrEmpty(request.RedirectUri)
            || !redirectUriValidator.Validate(request.RedirectUri)
        )
        {
            Log.RedirectUriInvalid(logger, request.RedirectUri ?? "[null]");
            return BadRequest("Invalid redirect URI - must match allowed hosts or patterns");
        }

        HttpContext.Session.SetString("RedirectUri", request.RedirectUri);

        if (!string.IsNullOrEmpty(request.State))
        {
            HttpContext.Session.SetString("State", request.State);
        }

        if (!string.IsNullOrEmpty(request.Tenant))
        {
            HttpContext.Session.SetString("Tenant", request.Tenant);
        }

        if (!string.IsNullOrEmpty(request.Scopes))
        {
            HttpContext.Session.SetString("Scopes", request.Scopes);
        }

        // Redirect to the Razor Page login page
        return Redirect("/Auth/Login");
    }

    /// <summary>
    /// Retrieves information about the currently authenticated user
    /// </summary>
    /// <returns>User information extracted from the JWT token claims</returns>
    /// <response code="200">Returns the user information</response>
    /// <response code="401">If the user is not authenticated</response>
    [Authorize]
    [HttpGet("api/me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var username = User.Identity?.Name;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        var tenant = User.FindFirstValue("tenant");
        var roles = User
            .Claims.Where(static c => c.Type is ClaimTypes.Role or "role")
            .Select(static c => c.Value)
            .ToList();

        var scopes = User
            .Claims.Where(static c => c.Type == "scope")
            .Select(static c => c.Value)
            .ToList();

        return Ok(
            new
            {
                UserId = userId,
                Username = username,
                Tenant = tenant,
                Roles = roles,
                Scopes = scopes,
            }
        );
    }

    /// <summary>
    /// Provides the public JSON Web Key Set (JWKS) for token validation
    /// </summary>
    /// <returns>A JSON Web Key Set containing the public key used to sign tokens</returns>
    /// <response code="200">Returns the JWKS</response>
    [HttpGet(".well-known/jwks.json")]
    [ProducesResponseType(typeof(JsonWebKeySet), StatusCodes.Status200OK)]
    public IActionResult GetJsonWebKeySet()
    {
        var jwks = new JsonWebKeySet();
        jwks.Keys.Add(keyManagementService.GetJsonWebKey());

        return Ok(jwks);
    }
}
