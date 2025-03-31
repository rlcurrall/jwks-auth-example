namespace AuthServer.Models;

/// <summary>
/// Response model containing the JWT token, refresh token, and expiration dates
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// The JWT token string
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The expiration date and time of the token
    /// </summary>
    /// <example>2025-03-30T14:30:00Z</example>
    public DateTime Expiration { get; set; }

    /// <summary>
    /// The refresh token string
    /// </summary>
    /// <example>6fd9a3f8-0e2b-4870-8567-7c5b09a8f267</example>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// The expiration date and time of the refresh token
    /// </summary>
    /// <example>2025-04-30T14:30:00Z</example>
    public DateTime? RefreshTokenExpiration { get; set; }
}
