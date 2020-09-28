namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IPluginManager
    {
        void Depends(IPluginAPI currentPlugin, string pluginName);
    }
}