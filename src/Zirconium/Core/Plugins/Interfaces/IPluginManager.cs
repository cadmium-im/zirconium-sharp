namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IPluginManager
    {
        dynamic Depends(IPluginAPI currentPlugin, string pluginName);
    }
}