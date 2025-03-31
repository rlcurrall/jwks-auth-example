using System.ComponentModel.DataAnnotations;

namespace AuthServer.Models;

/// <summary>
/// Request model for direct API-based authentication
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The username of the user
    /// </summary>
    /// <example>user1</example>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The password of the user
    /// </summary>
    /// <example>password123</example>
    [Required]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The tenant identifier
    /// </summary>
    /// <example>tenant1</example>
    [Required]
    public string Tenant { get; set; } = string.Empty;

    /// <summary>
    /// List of requested scopes for the token
    /// </summary>
    /// <example>["read", "write", "user_info"]</example>
    public List<string> Scopes { get; set; } = [];
}
