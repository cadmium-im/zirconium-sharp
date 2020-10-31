using System;

namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IPluginAPI
    {
        void Initialize(IPluginHostAPI pluginHost);
        void PreInitialize(IPluginManager pluginManager) {}
        string GetPluginUniqueName();
    }
}