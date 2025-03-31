using AuthServer.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace AuthServer.Extensions;

/// <summary>
/// Extension methods for JWT configuration
/// </summary>
public static class JwtConfigurationExtensions
{
    /// <summary>
    /// Configures JWT token validation using RSA keys from the AuthenticationService
    /// </summary>
    /// <param name="app">The WebApplication instance</param>
    /// <returns>The WebApplication instance for method chaining</returns>
    public static WebApplication ConfigureJwtValidation(this WebApplication app)
    {
        var keyManagementService = app.Services.GetRequiredService<IKeyManagementService>();

        var jwtOptions = app
            .Services.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        var parameters = jwtOptions.TokenValidationParameters;

        try
        {
            var jwk = keyManagementService.GetJsonWebKey();
            parameters.IssuerSigningKey = jwk;
            app.Logger.LogInformation("Successfully configured RSA key for JWT validation");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(
                ex,
                "Failed to configure JWT validation with RSA keys. Authentication will not work!"
            );
        }

        return app;
    }
}
