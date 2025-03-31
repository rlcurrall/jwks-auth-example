using AuthServer.Contracts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AuthServer.Services;

/// <summary>
/// Health check that verifies the KeyManagementService is properly initialized
/// and can provide a valid signing key.
/// </summary>
public class KeyManagementHealthCheck(
    IKeyManagementService keyManagementService,
    ILogger<KeyManagementHealthCheck> logger
) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var key = keyManagementService.GetSigningKey();
            var jwk = keyManagementService.GetJsonWebKey();

            if (string.IsNullOrEmpty(jwk.N) || string.IsNullOrEmpty(jwk.E))
            {
                return Task.FromResult(
                    HealthCheckResult.Degraded(
                        "RSA key parameters are invalid",
                        new InvalidOperationException("JWK missing required RSA parameters")
                    )
                );
            }

            logger.LogInformation("Key management health check passed successfully");

            return Task.FromResult(
                HealthCheckResult.Healthy("Key management service is properly initialized")
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Key management health check failed");

            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    "Key management service is not functioning properly",
                    ex
                )
            );
        }
    }
}
