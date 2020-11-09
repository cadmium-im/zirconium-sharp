using MongoDB.Driver;
using Log4Sharp;

namespace Zirconium.Core.Database
{
    public class DatabaseConnector
    {
        private App _app;

        public IMongoDatabase MongoDatabase { get; private set; }

        public DatabaseConnector(App app)
        {
            this._app = app;
            MongoClient client;

            string host = _app.Config.MongoDatabase.Host;
            int port = _app.Config.MongoDatabase.Port;
            string user = _app.Config.MongoDatabase.User;
            string password = _app.Config.MongoDatabase.Password;
            string database = _app.Config.MongoDatabase.Database;
            if ((user == null || user == "") && (password == null || password == ""))
            {
                client = new MongoClient($"mongodb://{host}:{port}");
            }
            else
            {
                client = new MongoClient($"mongodb://{user}:{password}@{host}:{port}");
            }
            var db = client.GetDatabase(database);
            this.MongoDatabase = db;
            Log.Debug("Database initialized");
        }
    }
}