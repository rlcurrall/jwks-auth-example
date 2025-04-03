using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthServer.Models;

/// <summary>
/// Represents a token request for OAuth 2.0 token endpoint
/// </summary>
public class TokenRequest
{
    /// <summary>
    /// The grant type being requested
    /// </summary>
    [Required]
    [JsonPropertyName("grant_type")]
    public string GrantType { get; set; } = string.Empty;

    /// <summary>
    /// The authorization code received from the authorization endpoint
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// The redirect URI used in the authorization request
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string? RedirectUri { get; set; }

    /// <summary>
    /// The refresh token used to obtain a new access token
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// The client ID
    /// </summary>
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }

    /// <summary>
    /// The client secret
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// The PKCE code verifier
    /// </summary>
    [JsonPropertyName("code_verifier")]
    public string? CodeVerifier { get; set; }
}
