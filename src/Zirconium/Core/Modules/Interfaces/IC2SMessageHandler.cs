using Zirconium.Core.Models;

namespace Zirconium.Core.Modules.Interfaces
{
    public interface IC2SMessageHandler {
        void HandleMessage(BaseMessage message);   
        bool IsAuthorizationRequired();
        string GetHandlerUniqueID();
    }
}