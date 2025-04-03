using System.Security.Cryptography;
using AuthServer.Contracts;
using AuthServer.Models;
using Microsoft.IdentityModel.Tokens;
using JWK = Microsoft.IdentityModel.Tokens.JsonWebKey;

namespace AuthServer.Services.Token;

/// <summary>
/// Service for managing cryptographic keys.
/// </summary>
public class KeyManagementService(
    ILogger<KeyManagementService> logger,
    TokenConfiguration tokenConfiguration
) : IKeyManagementService
{
    // Key used for signing JWT tokens
    private RsaSecurityKey? _signingKey;

    /// <summary>
    /// Gets the RSA security key used for signing JWT tokens.
    /// </summary>
    public RsaSecurityKey GetSigningKey()
    {
        if (_signingKey is not null)
        {
            return _signingKey;
        }

        var rsa = RSA.Create();

        rsa.FromXmlString(tokenConfiguration.PrivateKey);
        logger.LogInformation("Loaded existing RSA key from local file");

        _signingKey = new RsaSecurityKey(rsa);

        return _signingKey;
    }

    /// <summary>
    /// Gets the JSON Web Key representation of the public key for JWKS endpoint.
    /// </summary>
    public JWK GetJsonWebKey()
    {
        var signingKey = GetSigningKey();
        var rsaParameters = GetRsaParameters(signingKey, includePrivateParameters: false);

        // Generate a key ID (kid) - using a hash of the key modulus
        var kid = Convert.ToBase64String(SHA256.HashData(rsaParameters.Modulus ?? []))[..16];

        var jwk = new JWK
        {
            Kty = "RSA",
            Use = "sig",
            Kid = kid,
            Alg = "RS256",

            // RSA specific parameters - must be base64url encoded
            N = Base64UrlEncoder.Encode(rsaParameters.Modulus),
            E = Base64UrlEncoder.Encode(rsaParameters.Exponent),
        };

        return jwk;
    }

    /// <summary>
    /// Gets RSA parameters from a security key.
    /// </summary>
    private static RSAParameters GetRsaParameters(RsaSecurityKey key, bool includePrivateParameters)
    {
        if (key.Rsa is not null)
        {
            return key.Rsa.ExportParameters(includePrivateParameters);
        }

        // For .NET 8, the RsaSecurityKey.Parameters property has changed to not be nullable
        // Directly use the RSA instance since Parameters access is ambiguous
        var rsa = RSA.Create();
        rsa.ImportParameters(key.Parameters);
        return rsa.ExportParameters(includePrivateParameters);
    }
}
