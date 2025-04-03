namespace AuthServer.Contracts;

/// <summary>
/// Manages authorization codes used in the OAuth code flow
/// </summary>
public interface IAuthCodeRepository
{
    /// <summary>
    /// Creates a new authorization code and stores it with the provided data
    /// </summary>
    /// <param name="userId">The user ID the code is issued for</param>
    /// <param name="redirectUri">The redirect URI this code is valid for</param>
    /// <param name="scopes">The scopes requested for this code</param>
    /// <param name="codeChallenge">Optional PKCE code challenge for enhanced security</param>
    /// <param name="codeChallengeMethod">PKCE challenge method - either "S256" (recommended) or "plain"</param>
    /// <returns>The generated authorization code</returns>
    public string Create(
        string userId,
        string redirectUri,
        IEnumerable<string> scopes,
        string? codeChallenge = null,
        string? codeChallengeMethod = null
    );

    /// <summary>
    /// Validates and consumes an authorization code, returning the associated data
    /// </summary>
    /// <param name="code">The authorization code to redeem</param>
    /// <param name="redirectUri">The redirect URI to validate against</param>
    /// <returns>The data associated with the code, or null if invalid or already consumed</returns>
    public Task<AuthCodeData?> Consume(string code, string redirectUri);
}

/// <summary>
/// Data associated with an authorization code
/// </summary>
public class AuthCodeData
{
    /// <summary>
    /// The user ID associated with this code
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// The redirect URI this code was issued for
    /// </summary>
    public required string RedirectUri { get; set; }

    /// <summary>
    /// The scopes requested with this code
    /// </summary>
    public required IEnumerable<string> Scopes { get; set; }

    /// <summary>
    /// The PKCE code challenge (if provided during authorization)
    /// </summary>
    public string? CodeChallenge { get; set; }

    /// <summary>
    /// The PKCE code challenge method (S256 or plain)
    /// </summary>
    public string? CodeChallengeMethod { get; set; }

    /// <summary>
    /// When this code was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this code expires
    /// </summary>
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
}
