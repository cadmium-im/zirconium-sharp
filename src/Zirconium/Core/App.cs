using Zirconium.Core.Modules;
using Zirconium.Core.Modules.Interfaces;
using Zirconium.Core.Logging;

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

        public App(Config config)
        {
            Config = config;
            SessionManager = new SessionManager();
            Router = new Router(this);
            HostModuleAPI = new HostModuleAPI(this, Router);
            AuthManager = new AuthManager(this);
            Log.Info("Zirconium is initialized successfully");
        }
    }
}