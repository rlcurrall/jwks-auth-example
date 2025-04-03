using System.Net;

namespace AuthServer.Exceptions;

/// <summary>
/// The provided authorization grant or refresh token is invalid, expired, revoked,
/// does not match the redirection URI used in the authorization request, or was issued to another client.
/// </summary>
public class InvalidGrantException(
    string? message = null,
    string? errorDescription = null,
    string? errorUri = null
) : OAuthException(message, errorDescription, errorUri)
{
    public override string ErrorCode => "invalid_grant";
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
