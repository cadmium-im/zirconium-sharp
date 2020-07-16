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
    }

    public class Websocket
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Endpoint { get; set; }
    }
}