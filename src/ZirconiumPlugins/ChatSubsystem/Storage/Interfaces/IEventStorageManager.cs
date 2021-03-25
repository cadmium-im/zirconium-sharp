using System.Collections.Generic;
using System.Threading.Tasks;
using ChatSubsystem.Storage.Models;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Interfaces
{
    public interface IEventStorageManager
    {
        Task<(IList<Event>, IList<Chat>)> GetEventsForUser(EntityID user, EntityID since, int limit);
        Task<Event> GetEventById(EntityID id);
        Task SaveEvent(Event e);
    }
}