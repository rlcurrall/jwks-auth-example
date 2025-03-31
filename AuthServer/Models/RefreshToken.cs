namespace AuthServer.Models;

/// <summary>
/// Represents a refresh token for JWT renewal
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Unique identifier for the refresh token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The user ID associated with this token
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The username associated with this token
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The tenant associated with this token
    /// </summary>
    public string Tenant { get; set; } = string.Empty;

    /// <summary>
    /// The scopes associated with this token
    /// </summary>
    public List<string> Scopes { get; set; } = [];

    /// <summary>
    /// Token creation time
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Token expiration time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether this token has been revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Time when the token was revoked, if applicable
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Optional replacement token (for token rotation)
    /// </summary>
    public string? ReplacedByToken { get; set; }

    /// <summary>
    /// Check if the token is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    /// <summary>
    /// Check if the token is active
    /// </summary>
    public bool IsActive => !IsRevoked && !IsExpired;
}
