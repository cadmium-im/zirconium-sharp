namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IAuthProvider
    {
        // Method for checking validity of access token in each message
        SessionAuthData TestToken(string token);

        // Method for testing password when logging in
        bool TestPassword(string username, string pass);

        // User registration logic
        void CreateUser(string username, string pass);

        string CreateAuthToken(string entityID, string deviceID, long tokenExpirationMillis);
        string GetAuthProviderName();
    }
}