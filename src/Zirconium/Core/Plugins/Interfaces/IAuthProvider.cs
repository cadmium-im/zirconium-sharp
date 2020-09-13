namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IAuthProvider
    {
        bool TestToken(string token, JWTPayload payload);
        string GetAuthProviderName();
    }
}