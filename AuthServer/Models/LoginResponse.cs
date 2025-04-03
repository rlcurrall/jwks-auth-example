using System.Text.Json.Serialization;

namespace AuthServer.Models;

/// <summary>
/// Response model for successful authentication
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// The JWT access token
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// When the access token expires
    /// </summary>
    /// <example>2025-03-30T14:30:00Z</example>
    [JsonPropertyName("expires_in")]
    public DateTime Expiration { get; set; }

    /// <summary>
    /// The refresh token
    /// </summary>
    /// <example>6fd9a3f8-0e2b-4870-8567-7c5b09a8f267</example>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// When the refresh token expires
    /// </summary>
    /// <example>2025-04-30T14:30:00Z</example>
    [JsonPropertyName("refresh_token_expires_in")]
    public DateTime RefreshTokenExpiration { get; set; }

    /// <summary>
    /// The granted scopes, space-separated
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}
