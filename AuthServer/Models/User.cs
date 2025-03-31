namespace AuthServer.Models;

/// <summary>
/// Internal user model for authentication and authorization
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Username for login
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password hash for authentication
    /// </summary>
    /// <remarks>In a production environment, this would be a secure hash</remarks>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// The tenant that the user belongs to
    /// </summary>
    public string Tenant { get; set; } = string.Empty;

    /// <summary>
    /// The roles assigned to the user
    /// </summary>
    public List<string> Roles { get; set; } = [];
}
