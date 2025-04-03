using AuthServer.Models;

namespace AuthServer.Contracts;

/// <summary>
/// Repository for managing user data
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Validates credentials and returns a user if valid
    /// </summary>
    /// <param name="username">The username to validate</param>
    /// <param name="password">The password to validate</param>
    /// <param name="tenant">The tenant to validate for</param>
    /// <returns>A User object if valid, null otherwise</returns>
    public Task<User?> ValidateCredentials(string username, string password, string tenant);

    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="userId">The user ID to look up</param>
    /// <returns>A User object if found, null otherwise</returns>
    public Task<User?> GetUserById(string userId);

    /// <summary>
    /// Gets a user by their username in a specific tenant
    /// </summary>
    /// <param name="username">The username to look up</param>
    /// <param name="tenant">The tenant to look in</param>
    /// <returns>A User object if found, null otherwise</returns>
    public Task<User?> GetUserByUsername(string username, string tenant);
}
