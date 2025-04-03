using System.Net;

namespace AuthServer.Exceptions;

/// <summary>
/// The requested scope is invalid, unknown, or malformed.
/// </summary>
public class InvalidScopeException(
    string? message = null,
    string? errorDescription = null,
    string? errorUri = null
) : OAuthException(message, errorDescription, errorUri)
{
    public override string ErrorCode => "invalid_scope";
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
