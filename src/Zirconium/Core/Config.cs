using System.Collections.Generic;

namespace Zirconium.Core
{
    public class Config
    {
        // A list of enabled plugins (or extensions) in server
        public string[] EnabledPlugins { get; set; }

        // Server domain names (e.g. example.com)
        public string[] ServerDomains { get; set; }

        // Path to directory with plugin assemblies
        public string PluginsDirPath { get; set; }

        // ID of this server in terms of Cadmium federation network
        public string ServerID { get; set; }

        // Websocket server settings
        public Websocket Websocket { get; set; }

        // Database connection credentials
        public MongoDatabaseConfig MongoDatabase { get; set; }

        // Configurations of plugins
        public Dictionary<string, dynamic> Plugins { get; set; }

        public string AuthenticationProvider { get; set; }
    }

    public class Websocket
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Endpoint { get; set; }
    }

    public class MongoDatabaseConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}