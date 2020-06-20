namespace Zirconium.Core.Modules.Interfaces
{
    public interface IModuleAPI
    {
        void Initialize(IHostModuleAPI hostModuleAPI);
        string GetModuleUniqueName();
    }
}