using System.Collections.Concurrent;
using AuthServer.Contracts;
using AuthServer.Models;

namespace AuthServer.Services.Authentication;

/// <summary>
/// In-memory implementation of the user repository
/// </summary>
public class InMemoryUserRepository(ILogger<InMemoryUserRepository> logger) : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _usersById = new()
    {
        ["user1"] = new User
        {
            Id = "user1",
            Username = "user1",
            Tenant = "tenant1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1"),
            Roles = ["user"],
        },
        ["admin1"] = new User
        {
            Id = "admin1",
            Username = "admin",
            Tenant = "tenant1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpassword"),
            Roles = ["user", "admin"],
        },
        ["user2"] = new User
        {
            Id = "user2",
            Username = "user2",
            Tenant = "tenant2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password2"),
            Roles = ["user"],
        },
    };
    private readonly ConcurrentDictionary<string, string> _userKeysByUsername = new()
    {
        ["user1:tenant1"] = "user1",
        ["admin:tenant1"] = "admin1",
        ["user2:tenant2"] = "user2",
    };

    /// <summary>
    /// Validates user credentials
    /// </summary>
    public Task<User?> ValidateCredentials(string username, string password, string tenant)
    {
        // Construct the lookup key
        var lookupKey = GetLookupKey(username, tenant);

        // Try to find the user
        if (
            !_userKeysByUsername.TryGetValue(lookupKey, out var userId)
            || !_usersById.TryGetValue(userId, out var user)
        )
        {
            logger.LogWarning("User not found: {Username} in tenant {Tenant}", username, tenant);
            return Task.FromResult<User?>(null);
        }

        // Validate password - in production this would use BCrypt or similar
        if (!ValidatePassword(password, user.PasswordHash))
        {
            logger.LogWarning(
                "Invalid password for user: {Username} in tenant {Tenant}",
                username,
                tenant
            );
            return Task.FromResult<User?>(null);
        }

        logger.LogInformation(
            "User authenticated: {Username} in tenant {Tenant}",
            username,
            tenant
        );
        return Task.FromResult<User?>(user);
    }

    /// <summary>
    /// Gets a user by ID
    /// </summary>
    public Task<User?> GetUserById(string userId)
    {
        if (_usersById.TryGetValue(userId, out var user))
        {
            return Task.FromResult<User?>(user);
        }

        logger.LogWarning("User ID not found: {UserId}", userId);
        return Task.FromResult<User?>(null);
    }

    /// <summary>
    /// Gets a user by username in a tenant
    /// </summary>
    public Task<User?> GetUserByUsername(string username, string tenant)
    {
        var lookupKey = GetLookupKey(username, tenant);

        if (
            _userKeysByUsername.TryGetValue(lookupKey, out var userId)
            && _usersById.TryGetValue(userId, out var user)
        )
        {
            return Task.FromResult<User?>(user);
        }

        logger.LogWarning("User not found: {Username} in tenant {Tenant}", username, tenant);
        return Task.FromResult<User?>(null);
    }

    /// <summary>
    /// Creates a lookup key for username/tenant pairs
    /// </summary>
    private static string GetLookupKey(string username, string tenant)
    {
        return $"{username.ToLowerInvariant()}:{tenant.ToLowerInvariant()}";
    }

    /// <summary>
    /// Validates a password against a hash
    /// </summary>
    private static bool ValidatePassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
