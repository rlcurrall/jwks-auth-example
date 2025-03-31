using Microsoft.IdentityModel.Tokens;
using JWK = Microsoft.IdentityModel.Tokens.JsonWebKey;

namespace AuthServer.Contracts;

public interface IKeyManagementService
{
    public RsaSecurityKey GetSigningKey();
    public JWK GetJsonWebKey();
}
