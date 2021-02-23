using System.Collections.Generic;
using System.Threading.Tasks;
using ChatSubsystem.Storage.Models;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Interfaces
{
    public interface IEventStorageManager
    {
        IList<Event> GetEventsForUser(EntityID user, EntityID token, int limit);
        Task<Event> GetEventById(EntityID id);
        Task SaveEvent(Event e);
    }
}