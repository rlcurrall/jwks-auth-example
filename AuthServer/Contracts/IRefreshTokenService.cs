using AuthServer.Models;

namespace AuthServer.Contracts;

/// <summary>
/// Service for handling refresh token operations
/// </summary>
public interface IRefreshTokenService
{
    /// <summary>
    /// Generate a new refresh token for a user
    /// </summary>
    /// <param name="user">The user to generate the token for</param>
    /// <param name="scopes">The authorized scopes for this token</param>
    /// <returns>A new refresh token</returns>
    public RefreshToken GenerateRefreshToken(User user, List<string> scopes);

    /// <summary>
    /// Get a refresh token by its value
    /// </summary>
    /// <param name="token">The token string to find</param>
    /// <returns>The refresh token if found, null otherwise</returns>
    public Task<RefreshToken?> GetByToken(string token);

    /// <summary>
    /// Validate and update a refresh token (implements token rotation)
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>A new refresh token if valid, null otherwise</returns>
    public Task<RefreshToken?> RotateRefreshToken(string token);

    /// <summary>
    /// Revoke a specific refresh token
    /// </summary>
    /// <param name="token">The token to revoke</param>
    /// <returns>True if revoked successfully</returns>
    public Task<bool> Revoke(string token);

    /// <summary>
    /// Revoke all refresh tokens for a specific user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>True if revoked successfully</returns>
    public Task<bool> RevokeAllUserTokens(string userId);

    /// <summary>
    /// Remove expired tokens to clean up storage
    /// </summary>
    /// <returns>Number of tokens removed</returns>
    public Task<int> RemoveExpiredTokens();
}
