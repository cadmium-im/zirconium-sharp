namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IPluginAPI
    {
        void Initialize(IPluginHostAPI hostModuleAPI);
        string GetPluginUniqueName();
    }
}