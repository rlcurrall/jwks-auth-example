using System.Collections.Concurrent;
using AuthServer.Contracts;
using AuthServer.Models;

namespace AuthServer.Services.Client;

/// <summary>
/// In-memory implementation of the OAuth client repository
/// </summary>
public class InMemoryOAuthClientRepository(ILogger<InMemoryOAuthClientRepository> logger)
    : IOAuthClientRepository
{
    private readonly ConcurrentDictionary<string, OAuthClient> _clients = new()
    {
        ["weather-spa"] = new OAuthClient
        {
            ClientId = "weather-spa",
            ClientName = "Weather SPA",
            Description = "React Single Page Application for weather data",
            RedirectUris = ["http://localhost:5173/callback", "http://127.0.0.1:5173/callback"],
            AllowedScopes = ["openid", "profile", "email", "weather.read"],
            IsPublicClient = true,
        },
        ["default-client"] = new OAuthClient
        {
            ClientId = "default-client",
            ClientName = "Default Client",
            Description = "Default client for backward compatibility",
            RedirectUris = ["http://localhost:5173/callback", "http://127.0.0.1:5173/callback"],
            AllowedScopes = ["openid", "profile", "email", "weather.read"], // Weather SPA
            IsPublicClient = true,
        },
    };

    /// <summary>
    /// Gets a client by its ID
    /// </summary>
    public Task<OAuthClient?> GetClientByIdAsync(string clientId)
    {
        _ = _clients.TryGetValue(clientId, out var client);
        return Task.FromResult(client);
    }

    /// <summary>
    /// Creates a new client
    /// </summary>
    public Task<OAuthClient> CreateClientAsync(OAuthClient client)
    {
        if (_clients.TryAdd(client.ClientId, client))
        {
            logger.LogInformation("Created client: {ClientId}", client.ClientId);
            return Task.FromResult(client);
        }

        logger.LogWarning("Failed to create client: {ClientId} - already exists", client.ClientId);
        throw new InvalidOperationException($"Client with ID {client.ClientId} already exists");
    }

    /// <summary>
    /// Updates an existing client
    /// </summary>
    public Task<bool> UpdateClientAsync(OAuthClient client)
    {
        var result = _clients.TryUpdate(
            client.ClientId,
            client,
            _clients.GetOrAdd(client.ClientId, client)
        );
        logger.LogInformation("Updated client: {ClientId} - {Result}", client.ClientId, result);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Deletes a client
    /// </summary>
    public Task<bool> DeleteClientAsync(string clientId)
    {
        var result = _clients.TryRemove(clientId, out _);
        logger.LogInformation("Deleted client: {ClientId} - {Result}", clientId, result);
        return Task.FromResult(result);
    }

    /// <summary>
    /// Gets all clients
    /// </summary>
    public Task<List<OAuthClient>> GetAllClientsAsync()
    {
        return Task.FromResult(_clients.Values.ToList());
    }

    /// <summary>
    /// Validates a redirect URI for a client
    /// </summary>
    public Task<bool> ValidateRedirectUriAsync(string clientId, string redirectUri)
    {
        if (!_clients.TryGetValue(clientId, out var client))
        {
            logger.LogWarning("Client not found: {ClientId}", clientId);

            // For backward compatibility, allow any redirect URI if the client is not found
            if (clientId == "default-client")
            {
                logger.LogInformation(
                    "Using default client for redirect URI validation: {RedirectUri}",
                    redirectUri
                );
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        if (!client.IsActive)
        {
            logger.LogWarning("Client is inactive: {ClientId}", clientId);
            return Task.FromResult(false);
        }

        var isValid = client.RedirectUris.Contains(redirectUri);
        logger.LogInformation(
            "Redirect URI validation for client {ClientId}: {RedirectUri} - {Result}",
            clientId,
            redirectUri,
            isValid
        );

        return Task.FromResult(isValid);
    }
}
