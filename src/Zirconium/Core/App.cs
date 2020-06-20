using Zirconium.Core.Modules;
using Zirconium.Core.Modules.Interfaces;

namespace Zirconium.Core
{
    public class App
    {
        public SessionManager SessionManager { get; }
        public Router Router { get; }
        public ModuleManager ModuleManager { get; }
        public IHostModuleAPI HostModuleAPI { get; }

        public App()
        {
            SessionManager = new SessionManager();
            Router = new Router(this);
            HostModuleAPI = new HostModuleAPI(Router);
        }
    }
}