using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models;

/// <summary>
/// Represents an OAuth client application
/// </summary>
public class OAuthClient
{
    /// <summary>
    /// The unique identifier for the client
    /// </summary>
    public string ClientId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// The name of the client application
    /// </summary>
    [Required]
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// A description of the client application
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The list of redirect URIs allowed for this client
    /// </summary>
    public List<string> RedirectUris { get; set; } = [];

    /// <summary>
    /// The list of scopes this client is allowed to request
    /// </summary>
    public List<string> AllowedScopes { get; set; } = [];

    /// <summary>
    /// Whether this client is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is a public client (like a SPA) that cannot securely store a client secret
    /// </summary>
    public bool IsPublicClient { get; set; } = true;

    /// <summary>
    /// When this client was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
