using System.Text.RegularExpressions;
using AuthServer.Contracts;

namespace AuthServer.Services.Client;

/// <summary>
/// Service for validating redirect URIs to prevent open redirect vulnerabilities
/// </summary>
public class RedirectUriValidator(
    IConfiguration configuration,
    ILogger<RedirectUriValidator> logger
) : IRedirectUriValidator
{
    private readonly HashSet<string> _allowedHosts = new(
        configuration.GetSection("AllowedRedirectHosts").Get<string[]>()
            ?? ["localhost", "127.0.0.1"],
        StringComparer.OrdinalIgnoreCase
    );
    private readonly List<Regex> _allowedPatterns =
    [
        .. (configuration.GetSection("AllowedRedirectPatterns").Get<string[]>() ?? []).Select(
            static p => new Regex(p, RegexOptions.Compiled | RegexOptions.IgnoreCase)
        ),
    ];

    /// <summary>
    /// Validates if the provided redirect URI is allowed based on configured rules
    /// </summary>
    /// <param name="redirectUri">The URI to validate</param>
    /// <returns>True if the URI is allowed, false otherwise</returns>
    public bool Validate(string redirectUri)
    {
        if (string.IsNullOrEmpty(redirectUri))
        {
            return false;
        }

        try
        {
            var uri = new Uri(redirectUri);

            // First check if the host is in our allow list
            if (_allowedHosts.Contains(uri.Host))
            {
                return true;
            }

            // Then check against regex patterns
            if (_allowedPatterns.Any(pattern => pattern.IsMatch(uri.Host)))
            {
                return true;
            }

            // URI didn't match any allowed patterns
            logger.LogWarning("Rejected redirect URI: {RedirectUri}", redirectUri);
            return false;
        }
        catch (UriFormatException ex)
        {
            logger.LogWarning(ex, "Invalid redirect URI format: {RedirectUri}", redirectUri);
            return false;
        }
    }
}
