using Zirconium.Core.Plugins;
using Zirconium.Core.Plugins.Interfaces;
using Zirconium.Core.Logging;
using WebSocketSharp.Server;
using Zirconium.Core.Database;

namespace Zirconium.Core
{
    public class App
    {
        public Config Config;
        public SessionManager SessionManager { get; }
        public Router Router { get; }
        public PluginManager PluginManager { get; }
        public IPluginHostAPI PluginHostAPI { get; }
        public AuthManager AuthManager { get; }
        private WebSocketServer _websocketServer;
        public DatabaseConnector Database { get; private set; }

        public App(Config config)
        {
            Config = config;
            _websocketServer = new WebSocketServer($"ws://{config.Websocket.Host}:{config.Websocket.Port}");
            _websocketServer.AddWebSocketService<ConnectionHandler>(config.Websocket.Endpoint, () => new ConnectionHandler(this));
            SessionManager = new SessionManager();
            Router = new Router(this);
            PluginHostAPI = new PluginHostAPI(this, Router);
            AuthManager = new AuthManager(this);
            Database = new DatabaseConnector(this);
            PluginManager = new PluginManager(PluginHostAPI);
            PluginManager.LoadPlugins(config.PluginsDirPath, config.EnabledPlugins);
            AuthManager.SetDefaultAuthProvider();
            Log.Info("Zirconium is initialized successfully");
        }

        public void Run()
        {
            _websocketServer.Start();
        }

        public void Destroy()
        {
            Log.Info("Shutting down Zirconium...");
            _websocketServer.Stop();
        }
    }
}