using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Zirconium.Core;
using Zirconium.Core.Plugins.Interfaces;
using System.Linq;
using Newtonsoft.Json;
using JWT.Algorithms;
using JWT.Builder;
using System;
using Zirconium.Utils;

namespace DefaultAuthProvider
{
    public class DefaultAuthProviderPlugin : IPluginAPI
    {
        public string GetPluginUniqueName()
        {
            return "DefaultAuthProvider";
        }

        public void Initialize(IPluginHostAPI pluginHost)
        {
            var db = pluginHost.GetRawDatabase();
            var jwtSecret = pluginHost.GetSettings(this)["JWTSecret"];
            pluginHost.ProvideAuth(new DefaultAuthProvider(db, jwtSecret));
        }
    }

    public class DefaultAuthProvider : IAuthProvider
    {
        public class User
        {
            [BsonId]
            public ObjectId Id { get; set; }
            public string Username { get; set; }
            public byte[] Password { get; set; }
            public byte[] Salt { get; set; }
        }

        private IMongoDatabase _db;
        private IMongoCollection<User> usersCol;
        private string jwtSecret;

        public DefaultAuthProvider(IMongoDatabase db, string jwtSecret)
        {
            this._db = db;
            this.jwtSecret = jwtSecret;
            this.usersCol = db.GetCollection<User>("default_auth_data");
            _createUsernameUniqueIndex();
        }

        private void _createUsernameUniqueIndex()
        {
            var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<User>("Username");
            var indexDefinition = new IndexKeysDefinitionBuilder<User>().Ascending(field);
            var createIndexModel = new CreateIndexModel<User>(indexDefinition, options);
            usersCol.Indexes.CreateOne(createIndexModel);
        }

        public void CreateUser(string username, string pass)
        {
            var user = new User();
            user.Username = username; // TODO add check on bad chars
            var hashed = PasswordHasher.CreatePasswordHash(pass);
            System.GC.Collect();
            user.Password = hashed.Item1;
            user.Salt = hashed.Item2;
            _db.GetCollection<User>("default_auth_data").InsertOne(user);
        }

        public string GetAuthProviderName()
        {
            return "default";
        }

        public bool TestPassword(string username, string pass)
        {
            var filter = Builders<User>.Filter.Eq("Username", username);
            var user = usersCol.Find(filter).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            var valid = PasswordHasher.VerifyHash(pass, user.Salt, user.Password);
            System.GC.Collect();
            return valid;
        }

        public SessionAuthData TestToken(string token)
        {
            var jsonPayload = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(this.jwtSecret)
                .MustVerifySignature()
                .Decode(token);
            var payload = JsonConvert.DeserializeObject<SessionAuthData>(jsonPayload);
            return payload; // TODO add enchanced token validation
        }

        public string CreateAuthToken(string entityID, string deviceID, long tokenExpirationMillis)
        {
            SessionAuthData payload = new SessionAuthData();
            payload.DeviceID = deviceID;
            payload.EntityID = new string[] { entityID };
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(this.jwtSecret)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMilliseconds(tokenExpirationMillis).ToUnixTimeSeconds())
                .AddClaims(payload.ToDictionary())
                .Encode();
        }
    }
}
