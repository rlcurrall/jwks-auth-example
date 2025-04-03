using System.Collections.Concurrent;
using AuthServer.Contracts;

namespace AuthServer.Services.Client;

/// <summary>
/// In-memory implementation of the authorization code repository
/// </summary>
public class InMemoryAuthCodeRepository(ILogger<InMemoryAuthCodeRepository> logger)
    : IAuthCodeRepository
{
    private readonly ConcurrentDictionary<string, AuthCodeData> _authCodes = new();

    /// <summary>
    /// Creates a new authorization code and stores it with the provided data
    /// </summary>
    public string Create(
        string userId,
        string redirectUri,
        IEnumerable<string> scopes,
        string? codeChallenge = null,
        string? codeChallengeMethod = null
    )
    {
        // Generate a random auth code
        var code = Convert
            .ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "")[..20];

        var authCodeData = new AuthCodeData
        {
            UserId = userId,
            RedirectUri = redirectUri,
            Scopes = scopes.ToList(),
            CodeChallenge = codeChallenge,
            CodeChallengeMethod = codeChallengeMethod,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10), // Authorization codes valid for 10 minutes
        };

        _authCodes[code] = authCodeData;

        // Log the full details of the created code
        logger.LogInformation(
            "Authorization code created: code={Code}, userId={UserId}, redirectUri={RedirectUri}, expiresAt={ExpiresAt}",
            code,
            userId,
            redirectUri,
            authCodeData.ExpiresAt
        );

        return code;
    }

    /// <summary>
    /// Validates and consumes an authorization code, returning the associated data
    /// </summary>
    public Task<AuthCodeData?> Consume(string code, string redirectUri)
    {
        // Log the attempt to consume the code
        logger.LogInformation(
            "Attempting to consume authorization code: code={Code}, redirectUri={RedirectUri}",
            code,
            redirectUri
        );

        // Try to get and remove the auth code from the dictionary
        if (!_authCodes.TryRemove(code, out var authCodeData))
        {
            logger.LogWarning(
                "Authorization code not found: code={Code}, redirectUri={RedirectUri}",
                code,
                redirectUri
            );
            return Task.FromResult<AuthCodeData?>(null);
        }

        // Verify the code hasn't expired
        if (authCodeData.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning(
                "Authorization code expired: code={Code}, userId={UserId}, redirectUri={RedirectUri}, expiresAt={ExpiresAt}",
                code,
                authCodeData.UserId,
                redirectUri,
                authCodeData.ExpiresAt
            );
            return Task.FromResult<AuthCodeData?>(null);
        }

        // Verify the redirect URI matches
        if (authCodeData.RedirectUri != redirectUri)
        {
            logger.LogWarning(
                "Authorization code redirect URI mismatch: code={Code}, expected={ExpectedRedirectUri}, actual={ActualRedirectUri}",
                code,
                authCodeData.RedirectUri,
                redirectUri
            );
            return Task.FromResult<AuthCodeData?>(null);
        }

        logger.LogInformation(
            "Authorization code consumed successfully: code={Code}, userId={UserId}, redirectUri={RedirectUri}",
            code,
            authCodeData.UserId,
            redirectUri
        );
        return Task.FromResult<AuthCodeData?>(authCodeData);
    }
}
