using System.Collections.Immutable;
using System.Collections.Generic;
using Zirconium.Core.Plugins.Interfaces;
using MongoDB.Driver;
using Zirconium.Core.Logging;

namespace MongoDBPlugin
{
    class Plugin : IPluginAPI
    {
        private IMongoDatabase _database;

        public dynamic GetExportedAPI()
        {
            return _database;
        }

        public System.Type[] GetExportedTypes()
        {
            // var mongoCore = Assembly.Load("MongoDB.Driver.Core");
            // var mongoBson = Assembly.Load("MongoDB.Bson");
            return new System.Type[] {typeof(IMongoDatabase)};
        }

        public string GetPluginUniqueName()
        {
            return "MongoDB";
        }

        public void Initialize(IPluginHostAPI pluginHostAPI)
        {
            var settingsDynamic = pluginHostAPI.GetSettings(this);
            var settings = (IImmutableDictionary<string, dynamic>) ((IDictionary<string, object>)settingsDynamic).ToImmutableDictionary();

            var host = (string) settings.GetValueOrDefault("Host");
            var port = (int) settings.GetValueOrDefault("Port");
            var user = (string) settings.GetValueOrDefault("User");
            var password = (string) settings.GetValueOrDefault("Password");
            var database = (string) settings.GetValueOrDefault("Database");

            MongoClient client;
            if (user == null && password == null)
            {
                client = new MongoClient($"mongodb://{host}:{port}");
            }
            else
            {
                client = new MongoClient($"mongodb://{user}:{password}@{host}:{port}");
            }

            var db = client.GetDatabase(database);
            Log.Info("MongoDB is connected");
            _database = db;
        }

        public void PreInitialize(IPluginManager pluginManager) { }
    }
}
