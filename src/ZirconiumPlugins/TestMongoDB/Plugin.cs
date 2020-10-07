using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Zirconium.Core.Logging;
using Zirconium.Core.Plugins.Interfaces;

namespace TestMongoDB
{
    class Plugin : IPluginAPI
    {

        public string GetPluginUniqueName()
        {
            return "TestMongoDB";
        }

        public void Initialize(IPluginHostAPI pluginHostAPI)
        {
            var tm = new TestModel();
            tm.ABC = "qweqowie";
            var db = pluginHostAPI.GetRawDatabase();

            db.GetCollection<TestModel>("test_model").InsertOne(tm);
            Log.Debug("successfully inserted the model");
            var filter = Builders<TestModel>.Filter.Eq("ABC", "qweqowie");
            var found = db.GetCollection<TestModel>("test_model").Find(filter).FirstOrDefault();
            if (found != null) {
                Log.Debug($"Found document with ABC property: {found.ABC}");
            } else {
                Log.Debug("Nothing found!");
            }
        }

        public void PreInitialize(IPluginManager pluginManager)
        {
            
        }
    }

    class TestModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string ABC { get; set; }
    }
}
