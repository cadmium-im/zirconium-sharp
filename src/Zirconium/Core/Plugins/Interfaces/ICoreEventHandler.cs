using Zirconium.Core.Models;

namespace Zirconium.Core.Plugins.Interfaces
{
    public interface ICoreEventHandler
    {
        string GetHandlingEventType();
        void HandleEvent(CoreEvent coreEvent);
        string GetHandlerUniqueID();
    }
}