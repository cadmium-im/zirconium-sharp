using Zirconium.Core.Models;

namespace Zirconium.Core.Modules.Interfaces
{
    public interface ICoreEventHandler
    {
        string GetHandlingEventType();
        void HandleEvent(CoreEvent coreEvent);
        string GetHandlerUniqueID();
    }
}