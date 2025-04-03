using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthServer.Contracts;
using AuthServer.Models;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services.Authentication;

public class AuthenticationService(
    IKeyManagementService keyManagementService,
    TokenConfiguration tokenConfiguration,
    IRefreshTokenService refreshTokenService,
    IUserRepository userRepository
) : IAuthenticationService
{
    public async Task<User?> AttemptLogin(string username, string password, string tenant)
    {
        return await userRepository.ValidateCredentials(username, password, tenant);
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

        // Get the complete user data from the repository
        var user = await userRepository.GetUserById(rotatedToken.UserId);

        if (user == null)
        {
            // This should not happen in normal operation
            return (null, null);
        }

        var accessToken = GenerateToken(user, rotatedToken.Scopes);

        return (accessToken, rotatedToken);
    }
}
