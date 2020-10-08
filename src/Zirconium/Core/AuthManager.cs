using System.Collections.Generic;
using System;
using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;
using Zirconium.Utils;
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
            JWTPayload payload = new JWTPayload();
            payload.DeviceID = deviceID;
            payload.EntityID = entityID;
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(_secretString)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMilliseconds(tokenExpirationMillis).ToUnixTimeSeconds())
                .AddClaims(payload.ToDictionary())
                .Encode();
        }

        public string CreateToken(string entityID, string deviceID)
        {
            return CreateToken(entityID, deviceID, DEFAULT_TOKEN_EXPIRATION_TIME_HOURS);
        }

        public JWTPayload ValidateToken(string token)
        {
            var jsonPayload = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(_secretString)
                .MustVerifySignature()
                .Decode(token);
            var payload = JsonConvert.DeserializeObject<JWTPayload>(jsonPayload);
            if (DefaultAuthProvider == null)
            {
                throw new Exception("Default auth provider isn't specified");
            }
            var validToken = DefaultAuthProvider.TestToken(token, payload);
            if (!validToken)
                return null;
            return payload;
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