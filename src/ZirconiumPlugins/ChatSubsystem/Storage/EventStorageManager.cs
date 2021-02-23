using System.Collections.Generic;
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

        public IList<Event> GetEventsForUser(EntityID user, EntityID token, int limit)
        {
            return new List<Event>();
        }

        public async Task SaveEvent(Event e)
        {
            var connectFromField = (FieldDefinition<Event, EntityID>)"_id";
            var connectToField = (FieldDefinition<Event, EntityID>)"PrevID";
            var startWith = (AggregateExpressionDefinition<Event, string>)"$_id";
            var @as = (FieldDefinition<EventWithChildren, IEnumerable<Event>>)"Children";

            var res = await events.Aggregate()
                .GraphLookup(events, connectFromField, connectToField, startWith, @as)
                .Match("{ Children: { $size: 0 } }").FirstOrDefaultAsync();
            if (res == null)
            {
                await events.InsertOneAsync(e);
                return;
            }
            // TODO link to previous events in the room
            res.PrevID = res.Id;
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