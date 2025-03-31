using AuthServer.Models;

namespace AuthServer.Contracts;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Attempt to log in a user with the provided credentials
    /// </summary>
    /// <param name="username">The user's username</param>
    /// <param name="password">The user's password</param>
    /// <param name="tenant">The tenant the user belongs to</param>
    /// <returns>The user object if authentication succeeds, null otherwise</returns>
    public Task<User?> AttemptLogin(string username, string password, string tenant);

    /// <summary>
    /// Generate a JWT token for the specified user with the requested scopes
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="scopes">The requested access scopes</param>
    /// <returns>A JWT token string</returns>
    public string GenerateToken(User user, List<string> scopes);

    /// <summary>
    /// Generate both an access token and refresh token for the specified user
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="scopes">The requested access scopes</param>
    /// <returns>Tuple containing access token and refresh token</returns>
    public (string AccessToken, RefreshToken RefreshToken) GenerateTokenWithRefresh(
        User user,
        List<string> scopes
    );

    /// <summary>
    /// Refresh an access token using a refresh token
    /// </summary>
    /// <param name="refreshToken">The refresh token</param>
    /// <returns>Tuple containing new access token and refresh token, or null if refresh failed</returns>
    public Task<(string? AccessToken, RefreshToken? RefreshToken)> RefreshToken(
        string refreshToken
    );
}
