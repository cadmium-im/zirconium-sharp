using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatSubsystem.Storage.Interfaces;
using ChatSubsystem.Storage.Models;
using MongoDB.Driver;
using Zirconium.Core.Models;

namespace ChatSubsystem.Storage
{
    public class EventStorageManager : IEventStorageManager
    {
        private const string EventsCollectionName = "events";
        
        // get event by id
        // get events for user after some point in timeline (via sorting by event id)

        private ChatStorageManager _chatStorageManager;
        private IMongoCollection<Event> events;

        public EventStorageManager(ChatStorageManager chatStorageManager, IMongoDatabase database)
        {
            _chatStorageManager = chatStorageManager;
            events = database.GetCollection<Event>(EventsCollectionName);
        }

        public async Task<IList<Event>> GetEventsForUser(EntityID user, EntityID since, int limit)
        {
            var chats = await _chatStorageManager.GetChatsForUser(user);
            
            return new List<Event>();
        }

        private async Task<IList<Event>> GetEventsForChat(EntityID since, int limit)
        {
            return new List<Event>();
        }

        public async Task SaveEvent(Event e)
        {
            var connectFromField = (FieldDefinition<Event, EntityID>)"_id";
            var connectToField = (FieldDefinition<Event, EntityID>)"PrevID";
            var startWith = (AggregateExpressionDefinition<Event, string>)"$_id";
            var @as = (FieldDefinition<EventWithChildren, IEnumerable<Event>>)"Children";

            // link to previous global events
            var res = await events.Aggregate()
                .GraphLookup(events, connectFromField, connectToField, startWith, @as)
                .Match("{ Children: { $size: 0 } }").ToListAsync();
            if (res.Count == 0)
            {
                await events.InsertOneAsync(e);
                return;
            }
            res.ForEach(x =>
            {
                e.PrevID.Append(x.Id);
            });
            
            // link to previous events in chat
            connectToField = (FieldDefinition<Event, EntityID>)"PrevEvents";
            var opts = new AggregateGraphLookupOptions<Event, Event, EventWithChildren>()
            {
                RestrictSearchWithMatch = "{ ChatId: "+e.ChatId+" }"
            };
            res = await events.Aggregate()
                .GraphLookup(events, connectFromField, connectToField, startWith, @as, opts)
                .Match("{ Children: { $size: 0 } }").ToListAsync();
            if (res.Count == 0)
            {
                await events.InsertOneAsync(e);
                return;
            }
            res.ForEach(x =>
            {
                e.PrevEvents.Append(x.EventID);
            });
            
            await events.InsertOneAsync(e);
        }

        public async Task<Event> GetEventById(EntityID id)
        {
            var filter =
                Builders<Event>.Filter.Where(
                    x => x.Id.ToString() == id.LocalPart && x.OriginServer.ServerPart == id.ServerPart);
            var res = await events.FindAsync(filter);
            var ev = res.FirstOrDefault();
            return ev;
        }
    }
}