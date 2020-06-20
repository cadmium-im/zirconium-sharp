using System;
using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json;
using Zirconium.Utils;

namespace Zirconium.Core
{
    public class AuthManager
    {
        private App _app;
        private string _secretString;
        private const int TOKEN_EXPIRATION_TIME_HOURS = 24;

        public AuthManager(App app)
        {
            _app = app;
            _secretString = Guid.NewGuid().ToString();
        }

        public string CreateToken(string entityID, string deviceID)
        {
            JWTPayload payload = new JWTPayload();
            payload.DeviceID = deviceID;
            payload.EntityID = entityID;
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(_secretString)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(TOKEN_EXPIRATION_TIME_HOURS).ToUnixTimeSeconds())
                .AddClaims(payload.ToDictionary())
                .Encode();
        }

        public JWTPayload ValidateToken(string token)
        {
            var jsonPayload = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(_secretString)
                .MustVerifySignature()
                .Decode(token);
            return JsonConvert.DeserializeObject<JWTPayload>(jsonPayload);
        }
    }
}