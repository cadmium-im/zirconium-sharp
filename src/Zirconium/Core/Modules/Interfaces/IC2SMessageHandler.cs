using Zirconium.Core.Models;

namespace Zirconium.Core.Modules.Interfaces
{
    public interface IC2SMessageHandler {
        string GetHandlingMessageType();
        void HandleMessage(ConnectionInfo connInfo, BaseMessage message);   
        bool IsAuthorizationRequired();
        string GetHandlerUniqueID();
    }
}