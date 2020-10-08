namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IAuthProvider
    {
        // Method for checking validity of access token in each message
        bool TestToken(string token, JWTPayload payload);

        // Method for testing password when logging in
        bool TestPassword(string username, string pass);

        // User registration logic
        void CreateUser(string username, string pass);
        string GetAuthProviderName();
    }
}