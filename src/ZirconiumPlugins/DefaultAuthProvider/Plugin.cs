using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Zirconium.Core;
using Zirconium.Core.Plugins.Interfaces;
using Newtonsoft.Json;
using JWT.Algorithms;
using JWT.Builder;
using System;
using System.Collections.Generic;
using Zirconium.Core.Models;
using Zirconium.Core.Models.Authorization;
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
            pluginHost.ProvideAuth(new DefaultAuthProvider(db, jwtSecret, pluginHost.GetServerID()));
        }
    }

    public class DefaultAuthProvider : IAuthProvider
    {
        private const long DEFAULT_TOKEN_EXPIRATION_TIME_HOURS = 24 * 3600000;
        
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
        private string _serverID;

        public DefaultAuthProvider(IMongoDatabase db, string jwtSecret, string serverID)
        {
            this._db = db;
            this.jwtSecret = jwtSecret;
            this._serverID = serverID;
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

        public EntityID GetEntityID(IDictionary<string, dynamic> fieldsDict)
        {
            var fields = fieldsDict.ToObject<LoginPassFields>();
            return new EntityID('@', fields.Username, _serverID);
        }

        public void CreateUser(string username, string pass)
        {
            var user = new User();
            user.Username = username; // TODO add check on bad chars
            var hashed = PasswordHasher.CreatePasswordHash(pass);
            GC.Collect();
            user.Password = hashed.Item1;
            user.Salt = hashed.Item2;
            _db.GetCollection<User>("default_auth_data").InsertOne(user);
        }

        public (SessionAuthData, AuthorizationResponse) TestAuthFields(IDictionary<string, dynamic> fieldsDict)
        {
            if (fieldsDict.GetValueOrDefault("token") == null)
            {
                var fields = fieldsDict.ToObject<LoginPassFields>();
                bool valid = TestPassword(fields.Username, fields.Password);
                if (valid)
                {
                    // TODO Fix device system
                    var tokenData = CreateAuthToken(GetEntityID(fieldsDict).ToString(), "ABCDEF", DEFAULT_TOKEN_EXPIRATION_TIME_HOURS);
                    var res = new AuthorizationResponse()
                    {
                        Token = tokenData.Item1
                    };
                    return (tokenData.Item2, res);
                }

                return (null, null);
            }

            return (TestToken(fieldsDict["token"]), null);
        }

        public string GetAuthProviderName() => "default";

        public string[] GetAuthSupportedMethods() => new[] { "urn:cadmium:auth:login_password", "urn:cadmium:auth:token" };

        public bool TestPassword(string username, string pass)
        {
            var filter = Builders<User>.Filter.Eq("Username", username);
            var user = usersCol.Find(filter).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            var valid = PasswordHasher.VerifyHash(pass, user.Salt, user.Password);
            GC.Collect();
            return valid;
        }

        public SessionAuthData TestToken(string token)
        {
            try
            {
                var jsonPayload = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                    .WithSecret(jwtSecret)
                    .MustVerifySignature()
                    .Decode(token);
                var payload = JsonConvert.DeserializeObject<SessionAuthData>(jsonPayload);
                if (payload == null) payload = new SessionAuthData();
                return payload; // TODO add enchanced token validation
            }
            catch
            {
                return null;
            }
        }

        private (string, SessionAuthData) CreateAuthToken(string entityID, string deviceID, long tokenExpirationMillis)
        {
            SessionAuthData payload = new SessionAuthData();
            payload.DeviceID = deviceID;
            payload.EntityID = new string[] { entityID };
            return (new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                .WithSecret(this.jwtSecret)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMilliseconds(tokenExpirationMillis).ToUnixTimeSeconds())
                .AddClaims(payload.ToDictionary())
                .Encode(), payload);
        }
    }
}
