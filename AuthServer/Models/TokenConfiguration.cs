namespace AuthServer.Models;

public class TokenConfiguration
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInHours { get; set; } = 1;
    public string PrivateKey { get; set; } = string.Empty;
    public int RefreshExpiryInDays { get; set; } = 30;
    public int RefreshCleanupIntervalHours { get; set; } = 1;
}
