using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models;

/// <summary>
/// Request model for client registration
/// </summary>
public class ClientRegistrationRequest
{
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
    [Required]
    public List<string> RedirectUris { get; set; } = [];

    /// <summary>
    /// The list of scopes this client is allowed to request
    /// </summary>
    public List<string> AllowedScopes { get; set; } = [];

    /// <summary>
    /// Whether this is a public client (like a SPA) that cannot securely store a client secret
    /// </summary>
    public bool IsPublicClient { get; set; } = true;
}
