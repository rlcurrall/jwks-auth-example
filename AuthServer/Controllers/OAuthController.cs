using System.Security.Claims;
using AuthServer.Contracts;
using AuthServer.Exceptions;
using AuthServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Controllers;

/// <summary>
/// Controller for standard OAuth 2.0 and OpenID Connect endpoints
/// </summary>
[ApiController]
[Route("oauth")]
[Produces("application/json")]
public class OAuthController(
    IAuthenticationService authService,
    IRedirectUriValidator redirectUriValidator,
    IKeyManagementService keyManagementService,
    IAuthCodeRepository authCodeRepository,
    IUserRepository userRepository,
    IOAuthClientRepository clientRepository,
    TokenConfiguration tokenConfiguration,
    IRefreshTokenService refreshTokenService,
    ILogger<OAuthController> logger
) : ControllerBase
{
    private static readonly string[] _supportedResponseTypes = ["code"];
    private static readonly string[] _supportedSubjectTypes = ["public"];
    private static readonly string[] _supportedSigningAlgorithms = ["RS256"];
    private static readonly string[] _supportedScopes =
    [
        "openid",
        "profile",
        "email",
        "weather.read",
    ];
    private static readonly string[] _supportedTokenEndpointAuthMethods = ["none"];
    private static readonly string[] _supportedCodeChallengeMethods = ["S256", "plain"];
    private static readonly string[] _supportedClaims =
    [
        "sub",
        "iss",
        "aud",
        "exp",
        "iat",
        "name",
        "email",
        "role",
        "scope",
        "tenant",
    ];

    /// <summary>
    /// OAuth 2.0 Authorization Endpoint
    /// </summary>
    /// <returns>Redirects to the login page</returns>
    /// <response code="302">Redirects to the login page</response>
    /// <response code="400">If the request is invalid</response>
    [HttpGet("authorize")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Authorize(
        [FromQuery(Name = "client_id")] string? clientId,
        [FromQuery(Name = "redirect_uri")] string? redirectUri,
        [FromQuery(Name = "state")] string? state,
        [FromQuery(Name = "response_type")] string? responseType,
        [FromQuery(Name = "scope")] string? scope,
        [FromQuery(Name = "tenant")] string? tenant,
        [FromQuery(Name = "code_challenge")] string? codeChallenge,
        [FromQuery(Name = "code_challenge_method")] string? codeChallengeMethod,
        [FromQuery(Name = "nonce")] string? nonce
    )
    {
        // Log the incoming request parameters
        logger.LogInformation(
            "Authorization request received: client_id={ClientId}, redirect_uri={RedirectUri}, state={State}, scope={Scope}, response_type={ResponseType}",
            clientId ?? "[null]",
            redirectUri ?? "[null]",
            state ?? "[null]",
            scope ?? "[null]",
            responseType ?? "[null]"
        );

        // Validate required parameters
        if (string.IsNullOrEmpty(clientId))
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "client_id is required",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );
        }

        if (string.IsNullOrEmpty(redirectUri))
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "redirect_uri is required",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );
        }

        if (string.IsNullOrEmpty(responseType))
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "response_type is required",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );
        }

        // Validate response type
        if (responseType != "code")
        {
            throw new InvalidRequestException(
                "Unsupported Response Type",
                "Only 'code' response type is supported",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );
        }

        // Validate client
        var client =
            await clientRepository.GetClientByIdAsync(clientId)
            ?? throw new InvalidClientException(
                "Invalid Client",
                "Invalid client_id",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );

        // Validate redirect URI
        if (
            !redirectUriValidator.Validate(redirectUri)
            || !client.RedirectUris.Contains(redirectUri)
        )
        {
            logger.LogWarning("Invalid redirect URI: {RedirectUri}", redirectUri);
            throw new InvalidRequestException(
                "Invalid Redirect URI",
                "Invalid redirect URI - must match allowed hosts or patterns",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );
        }

        // Validate scopes
        var requestedScopes = string.IsNullOrEmpty(scope)
            ? []
            : scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (!client.AllowedScopes.ToHashSet().IsSupersetOf(requestedScopes.ToList()))
        {
            throw new InvalidScopeException(
                "Invalid Scope",
                "Requested scopes are not allowed for this client",
                "https://tools.ietf.org/html/rfc6749#section-4.1.2.1"
            );
        }

        // Store OAuth 2.0 parameters in session
        HttpContext.Session.SetString("RedirectUri", redirectUri);
        HttpContext.Session.SetString("ClientId", clientId);
        HttpContext.Session.SetString("ResponseType", responseType);

        if (!string.IsNullOrEmpty(state))
        {
            HttpContext.Session.SetString("State", state);
        }

        if (!string.IsNullOrEmpty(tenant))
        {
            HttpContext.Session.SetString("Tenant", tenant);
        }

        if (!string.IsNullOrEmpty(scope))
        {
            HttpContext.Session.SetString("Scopes", scope);
        }

        if (!string.IsNullOrEmpty(nonce))
        {
            HttpContext.Session.SetString("Nonce", nonce);
        }

        // Store PKCE parameters if provided
        if (!string.IsNullOrEmpty(codeChallenge))
        {
            HttpContext.Session.SetString("CodeChallenge", codeChallenge);

            if (!string.IsNullOrEmpty(codeChallengeMethod))
            {
                HttpContext.Session.SetString("CodeChallengeMethod", codeChallengeMethod);
            }
            else
            {
                // Default to S256 if not specified
                HttpContext.Session.SetString("CodeChallengeMethod", "S256");
            }
        }

        // Redirect to the Razor Page login page
        return Redirect("/Auth/Login");
    }

    /// <summary>
    /// OAuth 2.0 Token Endpoint
    /// </summary>
    /// <returns>A JWT token with a refresh token</returns>
    /// <response code="200">Returns the JWT token if exchange is successful</response>
    /// <response code="400">If the code/refresh token is invalid</response>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest,
        "application/json"
    )]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized,
        "application/json"
    )]
    public async Task<IActionResult> Token(
        [FromForm(Name = "grant_type")] string grantType,
        [FromForm(Name = "code")] string? code,
        [FromForm(Name = "redirect_uri")] string? redirectUri,
        [FromForm(Name = "refresh_token")] string? refreshToken,
        [FromForm(Name = "client_id")] string? clientId,
        [FromForm(Name = "client_secret")] string? clientSecret,
        [FromForm(Name = "code_verifier")] string? codeVerifier
    )
    {
        // Validate content type
        if (!Request.HasFormContentType)
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "Content-Type must be application/x-www-form-urlencoded",
                "https://tools.ietf.org/html/rfc6749#section-4.1.3"
            );
        }

        // Log the raw form data
        logger.LogInformation(
            "Raw form data received: {FormData}",
            string.Join(", ", Request.Form.Select(f => $"{f.Key}={f.Value}"))
        );

        // Create a TokenRequest from the form parameters
        var request = new TokenRequest
        {
            GrantType = grantType,
            Code = code,
            RedirectUri = redirectUri,
            RefreshToken = refreshToken,
            ClientId = clientId,
            ClientSecret = clientSecret,
            CodeVerifier = codeVerifier,
        };

        // Log the incoming request
        logger.LogInformation(
            "Token request received: grant_type={GrantType}, code={Code}, redirect_uri={RedirectUri}, refresh_token={RefreshToken}",
            request.GrantType,
            request.Code ?? "[null]",
            request.RedirectUri ?? "[null]",
            request.RefreshToken ?? "[null]"
        );

        var parsedGrantType = request.GrantType switch
        {
            "authorization_code" => GrantType.AuthorizationCode,
            "refresh_token" => GrantType.RefreshToken,
            _ => throw new InvalidRequestException(
                "Unsupported Grant Type",
                "Supported grant types are 'authorization_code' and 'refresh_token'",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            ),
        };

        return parsedGrantType switch
        {
            GrantType.AuthorizationCode => await HandleAuthorizationCodeGrant(request),
            GrantType.RefreshToken => await HandleRefreshTokenGrant(request),
            _ => throw new InvalidRequestException(
                "Unsupported Grant Type",
                "Supported grant types are 'authorization_code' and 'refresh_token'",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            ),
        };
    }

    /// <summary>
    /// OpenID Connect UserInfo Endpoint
    /// </summary>
    /// <returns>User information extracted from the JWT token claims</returns>
    /// <response code="200">Returns the user information</response>
    /// <response code="401">If the user is not authenticated</response>
    [Authorize]
    [HttpGet("userinfo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult UserInfo()
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
    /// OpenID Connect JWKS Endpoint
    /// </summary>
    /// <returns>A JSON Web Key Set containing the public key used to sign tokens</returns>
    /// <response code="200">Returns the JWKS</response>
    [HttpGet("/.well-known/jwks.json")]
    [ProducesResponseType(typeof(JsonWebKeySet), StatusCodes.Status200OK)]
    public IActionResult GetJsonWebKeySet()
    {
        var jwks = new JsonWebKeySet();
        jwks.Keys.Add(keyManagementService.GetJsonWebKey());

        return Ok(jwks);
    }

    /// <summary>
    /// OpenID Connect Discovery Document
    /// </summary>
    /// <returns>OpenID Connect configuration</returns>
    /// <response code="200">Returns the OpenID Connect configuration</response>
    [HttpGet("/.well-known/openid-configuration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetOpenIdConfiguration()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return Ok(
            new
            {
                issuer = baseUrl,
                authorization_endpoint = $"{baseUrl}/oauth/authorize",
                token_endpoint = $"{baseUrl}/oauth/token",
                userinfo_endpoint = $"{baseUrl}/oauth/userinfo",
                jwks_uri = $"{baseUrl}/oauth/.well-known/jwks.json",
                response_types_supported = _supportedResponseTypes,
                subject_types_supported = _supportedSubjectTypes,
                id_token_signing_alg_values_supported = _supportedSigningAlgorithms,
                scopes_supported = _supportedScopes,
                token_endpoint_auth_methods_supported = _supportedTokenEndpointAuthMethods,
                claims_supported = _supportedClaims,
                code_challenge_methods_supported = _supportedCodeChallengeMethods,
            }
        );
    }

    /// <summary>
    /// Handles the authorization_code grant type
    /// </summary>
    private async Task<IActionResult> HandleAuthorizationCodeGrant(TokenRequest request)
    {
        if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.RedirectUri))
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "Code and redirect_uri are required for authorization_code grant type",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            );
        }

        var codeData =
            await authCodeRepository.Consume(request.Code, request.RedirectUri)
            ?? throw new InvalidGrantException(
                "Invalid Grant",
                "Authorization code is invalid or expired",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            );

        if (!string.IsNullOrEmpty(codeData.CodeChallenge))
        {
            if (string.IsNullOrEmpty(request.CodeVerifier))
            {
                throw new InvalidRequestException(
                    "Invalid Request",
                    "Code verifier is required when PKCE was used in the authorization request",
                    "https://tools.ietf.org/html/rfc6749#section-5.2"
                );
            }

            if (
                !VerifyPkceCodeVerifier(
                    request.CodeVerifier,
                    codeData.CodeChallenge,
                    codeData.CodeChallengeMethod
                )
            )
            {
                throw new InvalidGrantException(
                    "Invalid Grant",
                    "PKCE verification failed - code_verifier is invalid",
                    "https://tools.ietf.org/html/rfc6749#section-5.2"
                );
            }
        }

        var user =
            await userRepository.GetUserById(codeData.UserId)
            ?? throw new InvalidGrantException(
                "Invalid Grant",
                "User associated with the authorization code could not be found",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            );

        var (accessToken, refreshToken) = authService.GenerateTokenWithRefresh(
            user,
            codeData.Scopes.ToList()
        );

        return Ok(
            new LoginResponse
            {
                Token = accessToken,
                Expiration = DateTime.Now.AddHours(tokenConfiguration.ExpiryInHours),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresAt,
                Scope = string.Join(" ", codeData.Scopes),
            }
        );
    }

    /// <summary>
    /// Handles the refresh_token grant type
    /// </summary>
    private async Task<IActionResult> HandleRefreshTokenGrant(TokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "Refresh token is required for refresh_token grant type",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            );
        }

        var (newAccessToken, newRefreshToken) = await authService.RefreshToken(
            request.RefreshToken
        );

        if (newAccessToken is null || newRefreshToken is null)
        {
            throw new InvalidGrantException(
                "Invalid Grant",
                "Invalid or expired refresh token",
                "https://tools.ietf.org/html/rfc6749#section-5.2"
            );
        }

        logger.LogInformation("Token refreshed for user: {Username}", newRefreshToken.Username);

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
    /// Verifies a PKCE code_verifier against the stored code_challenge
    /// </summary>
    private static bool VerifyPkceCodeVerifier(
        string codeVerifier,
        string codeChallenge,
        string? codeChallengeMethod
    )
    {
        // S256 is the default method if not specified
        codeChallengeMethod ??= "S256";

        if (codeChallengeMethod.Equals("S256", StringComparison.OrdinalIgnoreCase))
        {
            // SHA-256 transformation
            var verifierBytes = System.Text.Encoding.UTF8.GetBytes(codeVerifier);
            var challengeBytes = System.Security.Cryptography.SHA256.HashData(verifierBytes);
            var computedChallenge = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(
                challengeBytes
            );

            return computedChallenge == codeChallenge;
        }
        else if (codeChallengeMethod.Equals("plain", StringComparison.OrdinalIgnoreCase))
        {
            // Plain method - direct comparison
            return codeVerifier == codeChallenge;
        }

        // Unsupported challenge method
        return false;
    }

    /// <summary>
    /// OAuth 2.0 Token Revocation Endpoint
    /// </summary>
    /// <returns>Success response</returns>
    /// <response code="200">Token was successfully revoked or was already invalid</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost("revoke")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken(
        [FromForm(Name = "token")] string token,
        [FromForm(Name = "token_type_hint")] string? tokenTypeHint
    )
    {
        // Validate content type
        if (!Request.HasFormContentType)
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "Content-Type must be application/x-www-form-urlencoded",
                "https://tools.ietf.org/html/rfc7009#section-2.1"
            );
        }

        // Validate required parameters
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "token parameter is required",
                "https://tools.ietf.org/html/rfc7009#section-2.1"
            );
        }

        // If token_type_hint is provided, validate it
        if (!string.IsNullOrEmpty(tokenTypeHint) && tokenTypeHint != "refresh_token")
        {
            throw new InvalidRequestException(
                "Invalid Request",
                "Only refresh_token revocation is supported",
                "https://tools.ietf.org/html/rfc7009#section-2.1"
            );
        }

        // Try to revoke the token
        _ = await refreshTokenService.Revoke(token);

        // According to RFC 7009, we should return 200 OK even if the token was invalid
        logger.LogInformation("Token revocation request processed");
        return Ok();
    }
}
