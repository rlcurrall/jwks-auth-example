using System.Net;

namespace AuthServer.Exceptions;

/// <summary>
/// Client authentication failed (e.g., unknown client, no client authentication included,
/// or unsupported authentication method).
/// </summary>
public class InvalidClientException(
    string? message = null,
    string? errorDescription = null,
    string? errorUri = null
) : OAuthException(message, errorDescription, errorUri)
{
    public override string ErrorCode => "invalid_client";
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}
