using Zirconium.Core.Models;

namespace Zirconium.Core.Modules.Interfaces
{
    public interface IHostModuleAPI
    {
        void Hook(IC2SMessageHandler handler);
        void HookCoreEvent(ICoreEventHandler handler);
        void Unhook(IC2SMessageHandler handler);
        void UnhookCoreEvent(ICoreEventHandler handler);
        void FireEvent(CoreEvent coreEvent);
        string GenerateAuthToken(string entityID, string deviceID, int tokenExpirationMillis);
        string[] GetServerDomains();
        string GetServerID();
        void SendMessage(ConnectionInfo connInfo, BaseMessage message);
        dynamic GetSettings(IModuleAPI plugin);
        dynamic GetSettings(string pluginName);
    }
}