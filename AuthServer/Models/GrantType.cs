namespace AuthServer.Models;

/// <summary>
/// Supported OAuth 2.0 grant types
/// </summary>
public enum GrantType
{
    /// <summary>
    /// Authorization code grant - exchange a code for tokens
    /// </summary>
    AuthorizationCode,

    /// <summary>
    /// Refresh token grant - exchange a refresh token for a new access token
    /// </summary>
    RefreshToken,
}
