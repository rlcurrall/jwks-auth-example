namespace AuthServer.Models;

/// <summary>
/// Request model for refreshing an access token
/// </summary>
public class RefreshRequest
{
    /// <summary>
    /// The refresh token to use for obtaining a new access token
    /// </summary>
    /// <example>6fd9a3f8-0e2b-4870-8567-7c5b09a8f267</example>
    public string RefreshToken { get; set; } = string.Empty;
}
