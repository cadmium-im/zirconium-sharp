using System.Collections.Generic;
using System;
using Zirconium.Core.Plugins.Interfaces;
using System.Linq;

namespace Zirconium.Core
{
    public class AuthManager
    {
        private App _app;
        private string _secretString;
        private const long DEFAULT_TOKEN_EXPIRATION_TIME_HOURS = 24 * 3600000;
        private IList<IAuthProvider> _authProviders;
        public IAuthProvider DefaultAuthProvider { get; private set; }

        public AuthManager(App app)
        {
            _app = app;
            _authProviders = new List<IAuthProvider>();
            DefaultAuthProvider = null;
            _secretString = Guid.NewGuid().ToString();
        }

        public string CreateToken(string entityID, string deviceID, long tokenExpirationMillis)
        {
            if (DefaultAuthProvider == null) throw new Exception("Default auth provider isn't specified");
            return DefaultAuthProvider.CreateAuthToken(entityID, deviceID, tokenExpirationMillis);
        }

        public string CreateToken(string entityID, string deviceID)
        {
            return CreateToken(entityID, deviceID, DEFAULT_TOKEN_EXPIRATION_TIME_HOURS);
        }

        public SessionAuthData ValidateToken(string token)
        {
            if (DefaultAuthProvider == null) throw new Exception("Default auth provider isn't specified");
            return DefaultAuthProvider.TestToken(token);
        }

        public void AddAuthProvider(IAuthProvider provider)
        {
            _authProviders.Add(provider);
        }

        public void SetDefaultAuthProvider() {
            DefaultAuthProvider = _authProviders.Where(x => x.GetAuthProviderName() == _app.Config.AuthenticationProvider).FirstOrDefault();
        }
    }
}