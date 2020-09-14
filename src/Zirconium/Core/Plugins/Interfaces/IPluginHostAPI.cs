using Zirconium.Core.Models;

namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IPluginHostAPI
    {
        void Hook(IC2SMessageHandler handler);
        void HookCoreEvent(ICoreEventHandler handler);
        void Unhook(IC2SMessageHandler handler);
        void UnhookCoreEvent(ICoreEventHandler handler);
        void FireEvent(CoreEvent coreEvent);
        string GenerateAuthToken(string entityID, string deviceID, int tokenExpirationMillis);
        string[] GetServerDomains();
        string GetServerID();
        void SendMessage(Session session, BaseMessage message);
        dynamic GetSettings(IPluginAPI plugin);
        dynamic GetSettings(string pluginName);
        void ProvideAuth(IAuthProvider provider);
        IExposedSessionManager GetSessionManager();
    }
}