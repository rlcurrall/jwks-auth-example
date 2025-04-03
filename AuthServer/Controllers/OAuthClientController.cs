using AuthServer.Contracts;
using AuthServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers;

/// <summary>
/// Controller for OAuth client registration and management
/// </summary>
[ApiController]
[Route("api/clients")]
[Produces("application/json")]
public class OAuthClientController(
    IOAuthClientRepository clientRepository,
    ILogger<OAuthClientController> logger
) : ControllerBase
{
    /// <summary>
    /// Registers a new OAuth client
    /// </summary>
    /// <param name="request">The client registration request</param>
    /// <returns>The registered client</returns>
    /// <response code="201">Returns the newly created client</response>
    /// <response code="400">If the request is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(OAuthClient), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterClient([FromBody] ClientRegistrationRequest request)
    {
        // Validate request
        if (
            string.IsNullOrEmpty(request.ClientName)
            || request.RedirectUris == null
            || request.RedirectUris.Count == 0
        )
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Client name and at least one redirect URI are required",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7591#section-3.2.2",
                    Extensions = new Dictionary<string, object?>()
                    {
                        { "error", "invalid_client_metadata" },
                    },
                }
            );
        }

        // Create client
        var client = new OAuthClient
        {
            ClientName = request.ClientName,
            Description = request.Description,
            RedirectUris = request.RedirectUris,
            AllowedScopes =
                request.AllowedScopes.Count != 0
                    ? request.AllowedScopes
                    : ["openid", "profile", "email"],
            IsPublicClient = request.IsPublicClient,
        };

        try
        {
            // Save client
            var createdClient = await clientRepository.CreateClientAsync(client);

            logger.LogInformation(
                "Client registered: {ClientId} - {ClientName}",
                createdClient.ClientId,
                createdClient.ClientName
            );

            // Return client credentials (only shown once)
            return CreatedAtAction(
                nameof(GetClient),
                new { clientId = createdClient.ClientId },
                new
                {
                    client_id = createdClient.ClientId,
                    client_name = createdClient.ClientName,
                    redirect_uris = createdClient.RedirectUris,
                    allowed_scopes = createdClient.AllowedScopes,
                    is_public_client = createdClient.IsPublicClient,
                }
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = "Client Already Exists",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7591#section-3.2.2",
                    Extensions = new Dictionary<string, object?>()
                    {
                        { "error", "invalid_client_metadata" },
                    },
                }
            );
        }
    }

    /// <summary>
    /// Gets a client by ID
    /// </summary>
    /// <param name="clientId">The client ID</param>
    /// <returns>The client</returns>
    /// <response code="200">Returns the client</response>
    /// <response code="404">If the client is not found</response>
    [HttpGet("{clientId}")]
    [ProducesResponseType(typeof(OAuthClient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClient(string clientId)
    {
        var client = await clientRepository.GetClientByIdAsync(clientId);
        if (client == null)
        {
            return NotFound();
        }

        // Don't return sensitive information
        return Ok(
            new
            {
                client_id = client.ClientId,
                client_name = client.ClientName,
                description = client.Description,
                redirect_uris = client.RedirectUris,
                allowed_scopes = client.AllowedScopes,
                is_active = client.IsActive,
                is_public_client = client.IsPublicClient,
                created_at = client.CreatedAt,
            }
        );
    }

    /// <summary>
    /// Gets all clients
    /// </summary>
    /// <returns>A list of clients</returns>
    /// <response code="200">Returns the list of clients</response>
    [HttpGet]
    [Authorize(Roles = "Admin")] // Only admins can list all clients
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await clientRepository.GetAllClientsAsync();

        // Don't return sensitive information
        var clientList = clients.Select(static client => new
        {
            client_id = client.ClientId,
            client_name = client.ClientName,
            description = client.Description,
            redirect_uris = client.RedirectUris,
            allowed_scopes = client.AllowedScopes,
            is_active = client.IsActive,
            is_public_client = client.IsPublicClient,
            created_at = client.CreatedAt,
        });

        return Ok(clientList);
    }
}
