using System.Threading.Tasks;
using ChatSubsystem.Storage.Models;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage.Interfaces
{
    public interface IChatStorageManager
    {
        Task<Chat> GetPrivateChat(EntityID u1, EntityID u2);
        Task<Chat> GetById(EntityID chatId);
        Task<Event> GetEventById(EntityID id);
    }
}