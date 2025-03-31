namespace AuthServer.Contracts;

public interface IRedirectUriValidator
{
    public bool Validate(string redirectUri);
}
