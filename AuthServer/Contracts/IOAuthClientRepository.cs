using AuthServer.Models;

namespace AuthServer.Contracts;

/// <summary>
/// Repository for OAuth client applications
/// </summary>
public interface IOAuthClientRepository
{
    /// <summary>
    /// Gets a client by its ID
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <returns>The client, or null if not found</returns>
    public Task<OAuthClient?> GetClientByIdAsync(string clientId);

    /// <summary>
    /// Creates a new client
    /// </summary>
    /// <param name="client">The client to create</param>
    /// <returns>The created client</returns>
    public Task<OAuthClient> CreateClientAsync(OAuthClient client);

    /// <summary>
    /// Updates an existing client
    /// </summary>
    /// <param name="client">The client to update</param>
    /// <returns>True if the client was updated, false otherwise</returns>
    public Task<bool> UpdateClientAsync(OAuthClient client);

    /// <summary>
    /// Deletes a client
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <returns>True if the client was deleted, false otherwise</returns>
    public Task<bool> DeleteClientAsync(string clientId);

    /// <summary>
    /// Gets all clients
    /// </summary>
    /// <returns>A list of all clients</returns>
    public Task<List<OAuthClient>> GetAllClientsAsync();

    /// <summary>
    /// Validates a redirect URI for a client
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <param name="redirectUri">The redirect URI to validate</param>
    /// <returns>True if the redirect URI is valid for the client, false otherwise</returns>
    public Task<bool> ValidateRedirectUriAsync(string clientId, string redirectUri);
}
