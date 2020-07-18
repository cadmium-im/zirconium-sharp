using Zirconium.Core.Modules;
using Zirconium.Core.Modules.Interfaces;
using Zirconium.Core.Logging;
using WebSocketSharp.Server;

namespace Zirconium.Core
{
    public class App
    {
        public Config Config;
        public SessionManager SessionManager { get; }
        public Router Router { get; }
        public ModuleManager ModuleManager { get; }
        public IHostModuleAPI HostModuleAPI { get; }
        public AuthManager AuthManager { get; }
        private WebSocketServer _websocketServer;

        public App(Config config)
        {
            Config = config;
            _websocketServer = new WebSocketServer($"ws://{config.Websocket.Host}:{config.Websocket.Port}");
            _websocketServer.AddWebSocketService<ConnectionHandler>(config.Websocket.Endpoint, () => new ConnectionHandler(this));
            SessionManager = new SessionManager();
            Router = new Router(this);
            HostModuleAPI = new HostModuleAPI(this, Router);
            AuthManager = new AuthManager(this);
            ModuleManager = new ModuleManager(HostModuleAPI);
            ModuleManager.LoadModules(config.PluginsDirPath, config.EnabledPlugins);
            Log.Info("Zirconium is initialized successfully");
        }

        public void Run()
        {
            _websocketServer.Start();
        }

        public void Destroy() {
            Log.Info("Shutting down Zirconium...");
            _websocketServer.Stop();
        }
    }
}