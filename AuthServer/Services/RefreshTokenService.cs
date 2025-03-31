using System.Collections.Concurrent;
using AuthServer.Contracts;
using AuthServer.Models;

namespace AuthServer.Services;

public class RefreshTokenService(TokenConfiguration tokenConfiguration) : IRefreshTokenService
{
    // In-memory store for refresh tokens
    private readonly ConcurrentDictionary<string, RefreshToken> _refreshTokens = new();

    public RefreshToken GenerateRefreshToken(User user, List<string> scopes)
    {
        var token = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            Username = user.Username,
            Tenant = user.Tenant,
            Scopes = [.. scopes],
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenConfiguration.RefreshExpiryInDays),
        };

        // Store the token
        _refreshTokens[token.Token] = token;

        return token;
    }

    public Task<RefreshToken?> GetByToken(string token)
    {
        _ = _refreshTokens.TryGetValue(token, out var refreshToken);
        return Task.FromResult(refreshToken);
    }

    public async Task<RefreshToken?> RotateRefreshToken(string token)
    {
        var existingToken = await GetByToken(token);

        if (existingToken == null || !existingToken.IsActive)
        {
            return null;
        }

        // Create a user object to pass to GenerateRefreshToken
        var user = new User
        {
            Id = existingToken.UserId,
            Username = existingToken.Username,
            Tenant = existingToken.Tenant,
        };

        // Create a new token
        var newRefreshToken = GenerateRefreshToken(user, existingToken.Scopes);

        // Revoke the old token, pointing to the new one
        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newRefreshToken.Token;

        // Update the existing token in the dictionary
        _refreshTokens[token] = existingToken;

        return newRefreshToken;
    }

    public async Task<bool> RevokeRefreshToken(string token)
    {
        var existingToken = await GetByToken(token);

        if (existingToken == null)
        {
            return false;
        }

        // Revoke the token
        existingToken.IsRevoked = true;
        existingToken.RevokedAt = DateTime.UtcNow;

        // Update the token in the dictionary
        _refreshTokens[token] = existingToken;

        return true;
    }

    public Task<bool> RevokeAllUserTokens(string userId)
    {
        var userTokens = _refreshTokens.Values.Where(t => t.UserId == userId).ToList();

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _refreshTokens[token.Token] = token;
        }

        return Task.FromResult(true);
    }

    public Task<int> RemoveExpiredTokens()
    {
        var expiredTokens = _refreshTokens
            .Values.Where(static t => t.IsExpired || t.IsRevoked)
            .Select(static t => t.Token)
            .ToList();

        var count = 0;
        foreach (var token in expiredTokens)
        {
            if (_refreshTokens.TryRemove(token, out _))
            {
                count++;
            }
        }

        return Task.FromResult(count);
    }
}
