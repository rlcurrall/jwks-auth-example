using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthServer.Models;

/// <summary>
/// Request model for redirect-based authentication flow
/// </summary>
public class RedirectRequest
{
    /// <summary>
    /// The client identifier issued to the client during the registration process
    /// </summary>
    /// <example>weather-spa</example>
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    /// <summary>
    /// The URI to redirect back to after successful authentication
    /// </summary>
    /// <example>https://myapp.com/callback</example>
    [Required]
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// An opaque value used for preventing CSRF attacks
    /// </summary>
    /// <example>abc123</example>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Optional tenant identifier. If provided, tenant field will be pre-filled and hidden on the login form
    /// </summary>
    /// <example>tenant1</example>
    [JsonPropertyName("tenant")]
    public string? Tenant { get; set; }

    /// <summary>
    /// Optional scopes to request for the token. Scopes should be space-separated.
    /// </summary>
    /// <example>user_info profile email</example>
    [JsonPropertyName("scope")]
    public string? Scopes { get; set; }

    /// <summary>
    /// PKCE code challenge - SHA-256 hash of the code verifier (recommended) or plain text verifier
    /// </summary>
    /// <example>E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM</example>
    [JsonPropertyName("code_challenge")]
    public string? CodeChallenge { get; set; }

    /// <summary>
    /// PKCE code challenge method - "S256" (recommended) or "plain"
    /// </summary>
    /// <example>S256</example>
    [JsonPropertyName("code_challenge_method")]
    public string? CodeChallengeMethod { get; set; }
}
