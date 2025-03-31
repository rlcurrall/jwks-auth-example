using AuthServer.Contracts;
using AuthServer.Models;

namespace AuthServer.Services;

/// <summary>
/// Background service that periodically cleans up expired refresh tokens
/// </summary>
public class RefreshTokenCleanupService(
    IRefreshTokenService refreshTokenService,
    ILogger<RefreshTokenCleanupService> logger,
    TokenConfiguration tokenConfiguration
) : BackgroundService
{
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(
        tokenConfiguration.RefreshCleanupIntervalHours
    );

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Refresh token cleanup service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for the next cleanup cycle
                await Task.Delay(_cleanupInterval, stoppingToken);

                // Clean up expired tokens
                var removedCount = await refreshTokenService.RemoveExpiredTokens();

                logger.LogInformation("Removed {Count} expired refresh tokens", removedCount);
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during refresh token cleanup");
            }
        }

        logger.LogInformation("Refresh token cleanup service stopped");
    }
}
