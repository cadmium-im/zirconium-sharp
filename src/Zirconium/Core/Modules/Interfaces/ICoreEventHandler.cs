using Zirconium.Core.Models;

namespace Zirconium.Core.Modules.Interfaces
{
    public interface ICoreEventHandler
    {
        void HandleEvent(CoreEvent coreEvent);
        string GetHandlerUniqueID();
    }
}