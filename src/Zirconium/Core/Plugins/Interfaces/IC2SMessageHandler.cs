using Zirconium.Core.Models;

namespace Zirconium.Core.Plugins.Interfaces
{
    public interface IC2SMessageHandler {
        string GetHandlingMessageType();
        void HandleMessage(Session session, BaseMessage message);   
        bool IsAuthorizationRequired();
        string GetHandlerUniqueID();
    }
}