namespace AuthServer.Services.Infrastructure;

public static class Log
{
    private static readonly Action<ILogger, string, string, Exception?> _loginAttempt =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(1, nameof(LoginAttempt)),
            "Login attempt for user {Username} in tenant {Tenant}"
        );

    private static readonly Action<ILogger, string, string, Exception?> _loginFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            new EventId(2, nameof(LoginFailed)),
            "Login failed for user {Username} in tenant {Tenant}: Invalid credentials"
        );

    private static readonly Action<ILogger, string, string, Exception?> _loginSuccess =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(3, nameof(LoginSuccess)),
            "User {Username} authenticated successfully with scopes: {Scopes}"
        );

    private static readonly Action<ILogger, string, DateTime, Exception?> _tokenGenerated =
        LoggerMessage.Define<string, DateTime>(
            LogLevel.Debug,
            new EventId(4, nameof(TokenGenerated)),
            "Generated JWT token for {Username} with expiration {Expiration}"
        );

    private static readonly Action<ILogger, string, Exception?> _tokenRefreshed =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(5, nameof(TokenRefreshed)),
            "Refreshed JWT token for user {Username}"
        );

    private static readonly Action<ILogger, string, string, string, Exception?> _csrfTokenFailed =
        LoggerMessage.Define<string, string, string>(
            LogLevel.Warning,
            new EventId(6, nameof(CsrfTokenFailed)),
            "CSRF token validation failed in redirect login for user {Username}. Provided: {ProvidedToken}, Stored: {StoredToken}"
        );

    private static readonly Action<ILogger, string, Exception?> _redirectUriInvalid =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(7, nameof(RedirectUriInvalid)),
            "Invalid redirect URI attempted: {RedirectUri}"
        );

    private static readonly Action<ILogger, string, string, Exception?> _redirectLoginFailed =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            new EventId(8, nameof(RedirectLoginFailed)),
            "Redirect login failed for user {Username} in tenant {Tenant}: Invalid credentials"
        );

    private static readonly Action<ILogger, string, string, Exception?> _authCodeCreated =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(9, nameof(AuthCodeCreated)),
            "Authorization code created for user ID {UserId} with redirect URI {RedirectUri}"
        );

    private static readonly Action<ILogger, string, string, Exception?> _authCodeConsumed =
        LoggerMessage.Define<string, string>(
            LogLevel.Information,
            new EventId(10, nameof(AuthCodeConsumed)),
            "Authorization code consumed for user ID {UserId} with redirect URI {RedirectUri}"
        );

    private static readonly Action<ILogger, string, Exception?> _authCodeInvalid =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(11, nameof(AuthCodeInvalid)),
            "Invalid authorization code: {Code}"
        );

    private static readonly Action<ILogger, string, string, Exception?> _authCodeExpired =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            new EventId(12, nameof(AuthCodeExpired)),
            "Expired authorization code for user ID {UserId} with redirect URI {RedirectUri}"
        );

    private static readonly Action<ILogger, string, string, Exception?> _authCodeRedirectMismatch =
        LoggerMessage.Define<string, string>(
            LogLevel.Warning,
            new EventId(13, nameof(AuthCodeRedirectMismatch)),
            "Redirect URI mismatch for authorization code. Expected: {ExpectedUri}, Got: {ActualUri}"
        );

    public static void LoginAttempt(ILogger logger, string username, string tenant)
    {
        _loginAttempt(logger, username, tenant, null);
    }

    public static void LoginFailed(ILogger logger, string username, string tenant)
    {
        _loginFailed(logger, username, tenant, null);
    }

    public static void LoginSuccess(ILogger logger, string username, string scopes)
    {
        _loginSuccess(logger, username, scopes, null);
    }

    public static void TokenGenerated(ILogger logger, string username, DateTime expiration)
    {
        _tokenGenerated(logger, username, expiration, null);
    }

    public static void CsrfTokenFailed(
        ILogger logger,
        string username,
        string providedToken,
        string storedToken
    )
    {
        _csrfTokenFailed(logger, username, providedToken, storedToken, null);
    }

    public static void RedirectUriInvalid(ILogger logger, string redirectUri)
    {
        _redirectUriInvalid(logger, redirectUri, null);
    }

    public static void RedirectLoginFailed(ILogger logger, string username, string tenant)
    {
        _redirectLoginFailed(logger, username, tenant, null);
    }

    public static void TokenRefreshed(ILogger logger, string username)
    {
        _tokenRefreshed(logger, username, null);
    }

    public static void AuthCodeCreated(ILogger logger, string userId, string redirectUri)
    {
        _authCodeCreated(logger, userId, redirectUri, null);
    }

    public static void AuthCodeConsumed(ILogger logger, string userId, string redirectUri)
    {
        _authCodeConsumed(logger, userId, redirectUri, null);
    }

    public static void AuthCodeInvalid(ILogger logger, string code)
    {
        _authCodeInvalid(logger, code, null);
    }

    public static void AuthCodeExpired(ILogger logger, string userId, string redirectUri)
    {
        _authCodeExpired(logger, userId, redirectUri, null);
    }

    public static void AuthCodeRedirectMismatch(
        ILogger logger,
        string expectedUri,
        string actualUri
    )
    {
        _authCodeRedirectMismatch(logger, expectedUri, actualUri, null);
    }
}
