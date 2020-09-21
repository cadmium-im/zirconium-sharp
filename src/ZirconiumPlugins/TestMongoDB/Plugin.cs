using Zirconium.Core.Plugins.Interfaces;
using MongoDB.Driver;
using Zirconium.Core.Logging;
using System;

namespace TestMongoDB
{
    class Plugin : IPluginAPI
    {
        private IMongoDatabase _database;

        public dynamic GetExportedAPI()
        {
            return null;
        }

        public Type[] GetExportedTypes()
        {
            return null;
        }

        public string GetPluginUniqueName()
        {
            return "TestMongoDB";
        }

        public void Initialize(IPluginHostAPI pluginHostAPI)
        {
            var tm = new TestModel();
            tm.ABC = "qweqowie";
            _database.GetCollection<TestModel>("test_model").InsertOne(tm);
        }

        public void PreInitialize(IPluginManager pluginManager)
        {
            var db = pluginManager.Depends(this, "MongoDB");
            var receivedType = db.GetType();
            Log.Debug(db.GetType().FullName);
            Log.Debug(db.GetType().AssemblyQualifiedName);
            foreach (Type i in db.GetType().GetInterfaces()) {
                Log.Debug("DB object interface: " + i.AssemblyQualifiedName);
            }
            Log.Debug("Type info in current assembly context:");
            var mongoDBType = typeof(IMongoDatabase);
            Log.Debug(mongoDBType.FullName);
            Log.Debug(mongoDBType.AssemblyQualifiedName);
            Log.Debug(new Uri(mongoDBType.Module.Assembly.CodeBase).LocalPath);
            Log.Debug($"Casting compatibility: {mongoDBType.IsAssignableFrom(db.GetType())}");
            _database = (IMongoDatabase) db;
        }
    }

    class TestModel
    {
        public string ABC { get; set; }
    }
}
