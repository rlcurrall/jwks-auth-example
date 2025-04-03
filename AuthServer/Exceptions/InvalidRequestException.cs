using System.Net;

namespace AuthServer.Exceptions;

/// <summary>
/// The request is missing a required parameter, includes an unsupported parameter value,
/// or is otherwise malformed.
/// </summary>
public class InvalidRequestException(
    string? message = null,
    string? errorDescription = null,
    string? errorUri = null
) : OAuthException(message, errorDescription, errorUri)
{
    public override string ErrorCode => "invalid_request";
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
