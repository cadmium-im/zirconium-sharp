using System.Collections.Generic;
using System;
using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;
using Zirconium.Utils;
using Zirconium.Core.Plugins.Interfaces;

namespace Zirconium.Core
{
    public class AuthManager
    {
        private App _app;
        private string _secretString;
        private const long DEFAULT_TOKEN_EXPIRATION_TIME_HOURS = 24 * 3600000;
        private IList<IAuthProvider> _authProviders;
        private IAuthProvider _defaultAuthProvider;

        public AuthManager(App app)
        {
            _app = app;
            _authProviders = new List<IAuthProvider>();
            _defaultAuthProvider = null;
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

        public string CreateToken(string entityID, string deviceID) {
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
            if (_defaultAuthProvider == null) {
                throw new Exception("Default auth provider isn't specified");
            }
            var validToken = _defaultAuthProvider.TestToken(token, payload);
            if (!validToken)
                return null;
            return payload;
        }

        public void AddAuthProvider(IAuthProvider provider) {
            _authProviders.Add(provider);
        }
    }
}