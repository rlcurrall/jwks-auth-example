using System.Net;

namespace AuthServer.Exceptions;

/// <summary>
/// Base class for all OAuth 2.0 related exceptions
/// </summary>
public abstract class OAuthException(
    string? message = null,
    string? errorDescription = null,
    string? errorUri = null
) : Exception(message)
{
    /// <summary>
    /// The OAuth 2.0 error code as defined in RFC 6749
    /// </summary>
    public abstract string ErrorCode { get; }

    /// <summary>
    /// The HTTP status code to return
    /// </summary>
    public abstract HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Optional error description
    /// </summary>
    public string? ErrorDescription { get; } = errorDescription;

    /// <summary>
    /// Optional error URI
    /// </summary>
    public string? ErrorUri { get; } = errorUri;
}
