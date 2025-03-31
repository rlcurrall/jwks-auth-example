using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthServer.Contracts;
using AuthServer.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services;

public class AuthenticationService(
    IKeyManagementService keyManagementService,
    TokenConfiguration tokenConfiguration,
    IRefreshTokenService refreshTokenService
) : IAuthenticationService
{
    private readonly List<User> _users =
    [
        new User
        {
            Id = "1",
            Username = "user1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1"),
            Tenant = "tenant1",
            Roles = ["User"],
        },
        new User
        {
            Id = "2",
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpass"),
            Tenant = "tenant1",
            Roles = ["User", "Admin"],
        },
    ];

    public async Task<User?> AttemptLogin(string username, string password, string tenant)
    {
        var user = await Task.FromResult(
            _users.FirstOrDefault(u => u.Username == username && u.Tenant == tenant)
        );

        return user is not null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
            ? user
            : null;
    }

    public string GenerateToken(User user, List<string> scopes)
    {
        var signingKey = keyManagementService.GetSigningKey();

        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Name, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("tenant", user.Tenant),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim("role", role));
        }

        foreach (var scope in scopes)
        {
            claims.Add(new Claim("scope", scope));
        }

        var token = new JwtSecurityToken(
            issuer: tokenConfiguration.Issuer,
            audience: tokenConfiguration.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(tokenConfiguration.ExpiryInHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string AccessToken, RefreshToken RefreshToken) GenerateTokenWithRefresh(
        User user,
        List<string> scopes
    )
    {
        var accessToken = GenerateToken(user, scopes);

        var refreshToken = refreshTokenService.GenerateRefreshToken(user, scopes);

        return (accessToken, refreshToken);
    }

    public async Task<(string? AccessToken, RefreshToken? RefreshToken)> RefreshToken(
        string refreshToken
    )
    {
        var rotatedToken = await refreshTokenService.RotateRefreshToken(refreshToken);

        if (rotatedToken == null)
        {
            return (null, null);
        }

        var user = new User
        {
            Id = rotatedToken.UserId,
            Username = rotatedToken.Username,
            Tenant = rotatedToken.Tenant,
            // Roles are not stored in refresh token for security reasons
            Roles = [],
        };

        var accessToken = GenerateToken(user, rotatedToken.Scopes);

        return (accessToken, rotatedToken);
    }
}
