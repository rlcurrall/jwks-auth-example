using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models;

/// <summary>
/// Request model for redirect-based authentication flow
/// </summary>
public class RedirectRequest
{
    /// <summary>
    /// The URI to redirect back to after successful authentication
    /// </summary>
    /// <example>https://myapp.com/callback</example>
    [Required]
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// An opaque value used for preventing CSRF attacks
    /// </summary>
    /// <example>abc123</example>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Optional tenant identifier. If provided, tenant field will be pre-filled and hidden on the login form
    /// </summary>
    /// <example>tenant1</example>
    public string? Tenant { get; set; }

    /// <summary>
    /// Optional scopes to request for the token. Scopes should be space-separated.
    /// </summary>
    /// <example>user_info profile email</example>
    public string? Scopes { get; set; }
}
