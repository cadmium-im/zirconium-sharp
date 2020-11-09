using System;
using Zirconium.Core.Plugins.Interfaces;

namespace MessageStorage
{
    public class Plugin : IPluginAPI
    {
        public void Initialize(IPluginHostAPI pluginHost)
        {
            throw new NotImplementedException();
        }

        public string GetPluginUniqueName()
        {
            return "MessageStoragePlugin";
        }
    }
}
