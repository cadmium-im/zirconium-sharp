using InBandLogin.Handlers;
using Zirconium.Core.Plugins.Interfaces;

namespace InBandLogin
{
    public class Plugin : IPluginAPI
    {
        private IPluginHostAPI _pluginHost;

        public string GetPluginUniqueName()
        {
            return "InBandLogin";
        }

        public void Initialize(IPluginHostAPI pluginHost)
        {
            this._pluginHost = pluginHost;
            this._pluginHost.Hook(new LoginC2SHandler(this._pluginHost));
            this._pluginHost.Hook(new RegisterC2SHandler(this._pluginHost));
        }
    }
}
